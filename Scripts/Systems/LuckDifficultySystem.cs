using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class LuckDifficultySystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly ItemSettings _contentSettings;

        private ConfigEntity _configEntity;
        private EnvironmentEntity _memoryEntity;
        private bool _enabled;
        private int _lastGateScoreIndex;

        public LuckDifficultySystem(Contexts contexts, Services services, Settings settings) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.Random;
            _contentSettings = settings.ItemSettings;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.Item,
                         ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.Luck) continue;

                _configEntity = e;
                _configEntity.OnDestroyEntity += OnDestroyItemEntity;
                break;
            }

            if (_configEntity == null) return;

            _memoryEntity = _contexts.environment.generationMemoryEntity;

            _enabled = true;

            _lastGateScoreIndex = _memoryEntity.index.Value;
        }

        private void OnDestroyItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyItemEntity;
            _configEntity = null;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
            _contexts.environment.hasGenerationMemory;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (!_enabled) return;
            if (entities.Count == 0) return;

            _memoryEntity.generationMemory.DecreaseDifficulty = false;

            if (_configEntity == null)
            {
                _enabled = false;
                return;
            }

            int level = _configEntity.level.FullLevel;

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