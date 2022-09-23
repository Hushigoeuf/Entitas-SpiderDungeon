using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class CocoonGenerationSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly FlightSettings _flightSettings;
        private readonly ItemSettings _contentSettings;

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _cocoonEntity;
        private int _lastIndexBeforeSpawned;

        public CocoonGenerationSystem(Contexts contexts, Services services, Settings settings) :
            base(contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.Random;
            _flightSettings = settings.FlightSettings;
            _contentSettings = settings.ItemSettings;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.Item,
                         ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.Cocoon) continue;

                _cocoonEntity = e;
                _cocoonEntity.OnDestroyEntity += OnDestroyItemEntity;
                break;
            }

            if (_cocoonEntity == null) return;

            _memoryEntity = _contexts.environment.generationMemoryEntity;
        }

        private void OnDestroyItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyItemEntity;
            _memoryEntity = null;
            _cocoonEntity = null;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS && _contexts.environment.hasGenerationMemory;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (_cocoonEntity == null) return;
            if (entities.Count == 0) return;

            if (_contexts.config.mainConfigEntity.isGameOver) return;

            if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value >= _flightSettings.MaxLifeInStorage) return;

            if (_memoryEntity.index.Value < _contentSettings.CocoonPauseAfterStart) return;

            int level = _cocoonEntity.level.FullLevel;
            if (_memoryEntity.index.Value - _lastIndexBeforeSpawned <
                _contentSettings.CocoonDropPause.GetCount(level)) return;
            _lastIndexBeforeSpawned = _memoryEntity.index.Value;

            for (var ei = 0; ei < entities.Count; ei++)
            {
                if (!_randomService.IsChance(_contentSettings.CocoonDropChance.GetChance(level))) continue;

                _memoryEntity.generationMemory.PrefabList.Clear();
                _memoryEntity.generationMemory.PrefabList.Add(_contentSettings.CocoonPrefab);
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