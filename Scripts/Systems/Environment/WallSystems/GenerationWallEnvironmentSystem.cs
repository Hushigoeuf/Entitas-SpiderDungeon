using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationWallEnvironmentSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly WallData _data;
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;

        public GenerationWallEnvironmentSystem(Contexts contexts, Services services, WallData data) : base(
            contexts.environment)
        {
            _data = data;
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));
        }


        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_WALLS;
        }


        protected override void Execute(List<EnvironmentEntity> entities)
        {
            foreach (var entity in entities)
            {
                var wall = _poolService.Spawn(entity.pool.Value, _data.WallPrefab);
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
                    eDestroy.AddTransform(_cameraService.Container);
                    eDestroy.AddTarget(wall);
                    eDestroy.AddOffset(GameSettings.GetTopTransformPoint(_data.PixelsPerWall.ToUnits()));
                    eDestroy.AddPool(entity.pool.Value);
                }
            }
        }
    }
}