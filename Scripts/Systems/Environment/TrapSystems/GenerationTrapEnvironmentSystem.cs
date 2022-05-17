using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationTrapEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly ICameraService _cameraService;
        private readonly IPoolService _poolService;
        private readonly TrapData _trapData;

        public GenerationTrapEnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _cameraService = services.CameraService;
            _contexts = contexts;
            _poolService = services.PoolService;
            _trapData = data.TrapData;
        }

        public void Initialize()
        {
            var size = _trapData.GenerationStepSize;
            var count = Mathf.CeilToInt(GameSettings.TransformHeight / 2f / size);
            var eGeneration = _contexts.environment.CreateEntity();
            {
                eGeneration.isGeneration = true;
                eGeneration.AddTransform(_cameraService.Container);
                eGeneration.AddPosition(new Vector3(0, _cameraService.Container.position.y + count * size, 0));
                eGeneration.AddOffset(count * size);
                eGeneration.AddLength(size);
                eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_TRAPS);
                eGeneration.AddIndex(0);
            }
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
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
                   _contexts.environment.hasGenerationMemory;
        }


        protected override void Execute(List<EnvironmentEntity> entities)
        {
            var prefabs = _contexts.environment.generationMemory.PrefabList;
            for (var ei = 0; ei < entities.Count; ei++)
            for (var pi = 0; pi < prefabs.Count; pi++)
            {
                var trap = _poolService.Spawn(entities[ei].pool.Value, prefabs[pi]);
                trap.position = new Vector3(trap.position.x, entities[ei].position.Value.y, trap.position.z);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_cameraService.Container);
                    eDestroy.AddTarget(trap);
                    eDestroy.AddOffset(GameSettings.GetTopTransformPoint(10.24f));
                    eDestroy.AddPool(entities[ei].pool.Value);
                }
            }
        }
    }
}