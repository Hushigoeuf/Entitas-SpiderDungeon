using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class DiamondOnCollidingSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly DiamondSettings _diamondSettings;
        private readonly Transform _camera;

        private ConfigEntity _eStatConfig;

        public DiamondOnCollidingSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.environment)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _diamondSettings = settings.DiamondSettings;
            _camera = camera;
        }

        public void Initialize()
        {
            _eStatConfig = _contexts.config.statConfigEntity;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.Diamond);

        protected override bool Filter(EnvironmentEntity entity) => entity.hasTransform && entity.hasIndex;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;
            for (var i = 0; i < entities.Count; i++)
            {
                var explosion = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS,
                    _diamondSettings.ExplosionPrefab);
                explosion.position = new Vector3(entities[i].transform.Value.position.x,
                    entities[i].transform.Value.position.y, explosion.position.z);

                _poolService.Despawn(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS, entities[i].transform.Value);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_camera);
                    eDestroy.AddTarget(explosion);
                    eDestroy.AddOffset(ScreenSettings.GetTopScreenPoint(5.12f));
                    eDestroy.AddPool(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS);
                }

                var costSize = _diamondSettings.DefaultCostSize;
                if (entities[i].index.Value != 0)
                    costSize = _diamondSettings.OtherPrefabList[entities[i].index.Value - 1].CostSize;
                _eStatConfig.ReplaceDiamondCount(_eStatConfig.diamondCount.Value + costSize);
            }
        }
    }
}