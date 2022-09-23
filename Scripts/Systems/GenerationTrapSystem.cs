using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class GenerationTrapSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly TrapSettings _trapSettings;
        private readonly Transform _camera;

        public GenerationTrapSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.environment)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _trapSettings = settings.TrapSettings;
            _camera = camera;
        }

        public void Initialize()
        {
            float size = _trapSettings.GenerationStepSize;
            int count = Mathf.CeilToInt(ScreenSettings.CurrentTransformHeight / 2f / size);
            var eGeneration = _contexts.environment.CreateEntity();
            {
                eGeneration.isGeneration = true;
                eGeneration.AddTransform(_camera);
                eGeneration.AddPosition(new Vector3(0, _camera.position.y + count * size, 0));
                eGeneration.AddOffset(count * size);
                eGeneration.AddLength(size);
                eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_TRAPS);
                eGeneration.AddIndex(0);
            }
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
            _contexts.environment.hasGenerationMemory;

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
                    eDestroy.AddTransform(_camera);
                    eDestroy.AddTarget(trap);
                    eDestroy.AddOffset(ScreenSettings.GetTopScreenPoint(10.24f));
                    eDestroy.AddPool(entities[ei].pool.Value);
                }
            }
        }
    }
}