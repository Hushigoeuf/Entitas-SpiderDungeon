using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class CoconGenerationEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly FlightData _flightData;
        private readonly TrapData _trapData;
        private readonly ContentSettingsObject _contentSettings;

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _coconEntity;
        private int _lastIndexBeforeSpawned;

        public CoconGenerationEnvironmentSystem(Contexts contexts, Services services, Data data) : base(contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.RandomService;
            _flightData = data.FlightData;
            _trapData = data.TrapData;
            _contentSettings = data.ContentData;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.ItemBehaviourType)))
            {
                if (e.itemBehaviourType.Value != ItemBehaviourTypes.Resurrection) continue;
                _coconEntity = e;
                _coconEntity.OnDestroyEntity += _OnDestroyCoconEntity;
                break;
            }

            if (_coconEntity == null) return;

            _memoryEntity = _contexts.environment.generationMemoryEntity;
        }

        private void _OnDestroyCoconEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyCoconEntity;
            _memoryEntity = null;
            _coconEntity = null;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool));
        }

        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS && _contexts.environment.hasGenerationMemory;
        }

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (_coconEntity == null) return;
            if (entities.Count == 0) return;
            if (_contexts.config.mainConfigEntity.isGameOver) return;
            if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value >= _flightData.MaxLifeInStorage) return;
            if (_memoryEntity.index.Value < _contentSettings.CoconPauseAfterStart) return;

            var level = _coconEntity.level.FullLevel;
            if (_memoryEntity.index.Value - _lastIndexBeforeSpawned <
                _contentSettings.CoconDropPause.GetCount(level)) return;
            _lastIndexBeforeSpawned = _memoryEntity.index.Value;

            for (var ei = 0; ei < entities.Count; ei++)
            {
                if (!_randomService.IsChance(_contentSettings.CoconDropChance.GetChance(level))) continue;
                _memoryEntity.generationMemory.PrefabList.Clear();
                _memoryEntity.generationMemory.PrefabList.Add(_contentSettings.CoconPrefab);
                _memoryEntity.environmentPosition.Set(2);
                _memoryEntity.generationMemory.DiamondFreeSpaces[0] = false;
                _memoryEntity.generationMemory.DiamondFreeSpaces[1] = false;
                _memoryEntity.generationMemory.DiamondFreeSpaces[2] = false;
                _memoryEntity.generationMemory.DiamondFreeSpaces[3] = false;
                break;
            }
        }
    }
}