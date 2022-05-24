using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Создает стены по мере продвижения персонажей.
    /// </summary>
    public sealed class WallGenerationSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly WallSettings _settings;
        private readonly IPoolService _poolService;
        private readonly Transform _camera;

        public WallGenerationSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.environment)
        {
            _poolService = services.Pool;
            _settings = settings.WallSettings;
            _camera = camera;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_WALLS;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            foreach (var entity in entities)
            {
                var wall = _poolService.Spawn(entity.pool.Value, _settings.WallPrefab);
                wall.position = new Vector3(wall.position.x, entity.position.Value.y, wall.position.z);

                var eWall = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eWall.isGenerationTask = true;
                    eWall.AddPool(entity.pool.Value);
                    eWall.AddTarget(wall);
                    eWall.isCleanup = true;
                }

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_camera);
                    eDestroy.AddTarget(wall);
                    eDestroy.AddOffset(
                        ScreenSettings.GetTopScreenPoint(ScreenSettings.PixelsToUnits(_settings.PixelsPerWall)));
                    eDestroy.AddPool(entity.pool.Value);
                }
            }
        }
    }
}