using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class LuckEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly TrapData _trapData;
        private readonly ContentSettingsObject _contentSettings;

        private ConfigEntity _luckEntity;
        private EnvironmentEntity _memoryEntity;

        private float _defaultMinGateScore;
        private int _defaultMinGateCount;
        private bool _enabled;
        private int _lastGateScoreIndex;

        public LuckEnvironmentSystem(Contexts contexts, Services services, Data data) : base(contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.RandomService;
            _trapData = data.TrapData;
            _contentSettings = data.ContentData;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.ItemBehaviourType)))
            {
                if (e.itemBehaviourType.Value != ItemBehaviourTypes.Luck) continue;
                _luckEntity = e;
                _luckEntity.OnDestroyEntity += _OnDestroyLuckEntity;
                break;
            }

            if (_luckEntity == null) return;

            _memoryEntity = _contexts.environment.generationMemoryEntity;

            _enabled = true;

            _lastGateScoreIndex = _memoryEntity.index.Value;
        }

        private void _OnDestroyLuckEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyLuckEntity;
            _luckEntity = null;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool));
        }

        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
                   _contexts.environment.hasGenerationMemory;
        }

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (!_enabled) return;
            if (entities.Count == 0) return;

            _memoryEntity.generationMemory.DecreaseDifficulty = false;

            if (_luckEntity == null)
            {
                _enabled = false;
                return;
            }

            var level = _luckEntity.level.FullLevel;

            if (_memoryEntity.index.Value - _lastGateScoreIndex >
                _contentSettings.LuckDropPause.GetCount(level) &&
                _randomService.IsChance(_contentSettings.LuckDropChance.GetChance(level)))
            {
                _lastGateScoreIndex = _memoryEntity.index.Value;
                _memoryEntity.generationMemory.DecreaseDifficulty = true;
            }
        }
    }
}