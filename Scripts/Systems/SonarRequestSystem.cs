using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class SonarRequestSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly ChanceLevelField _sonarDropChance;
        private readonly CountLevelField _sonarDropCount;

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _sonarEntity;
        private int[] _memoryPosition;

        public SonarRequestSystem(Contexts contexts, Services services, Settings settings) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.Random;
            _sonarDropChance = settings.ItemSettings.SonarDropChance;
            _sonarDropCount = settings.ItemSettings.SonarDropCount;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.Item,
                         ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.Sonar) continue;

                _sonarEntity = e;
                _sonarEntity.OnDestroyEntity += _OnDestroySonarEntity;
                break;
            }

            if (_sonarEntity == null) return;

            _memoryEntity = _contexts.environment.generationMemoryEntity;
            _memoryPosition = new int[_memoryEntity.environmentPosition.GetSize()];
        }

        private void _OnDestroySonarEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroySonarEntity;
            _memoryEntity = null;
            _sonarEntity = null;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS && _contexts.environment.hasGenerationMemory;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (_sonarEntity == null) return;
            if (entities.Count == 0) return;

            _memoryEntity.environmentPosition.Fill(ref _memoryPosition);

            var level = _sonarEntity.level.FullLevel;

            for (var i = 0; i < entities.Count; i++)
            {
                if (!_randomService.IsChance(_sonarDropChance.GetChance(level))) continue;

                var direction = _randomService.Choose(-1, 1);
                var count = _sonarDropCount.GetCount(level);
                var position = new int[_memoryPosition.Length];

                var index = 0;
                for (var pi = 0; pi < position.Length; pi++)
                {
                    index = pi;
                    if (direction == 1) index = position.Length - pi - 1;
                    if (_memoryPosition[index] != 1) continue;
                    position[index] = 1;
                    count--;
                    if (count <= 0) break;
                }

                var e = _contexts.config.CreateEntity();
                {
                    e.isSonarRequest = true;
                    e.AddEnvironmentPosition(position[0], position[1], position[2], position[3]);
                    e.AddPosition(new Vector3(0, entities[i].position.Value.y - entities[i].length.Value / 2, 0));
                }
            }
        }
    }
}