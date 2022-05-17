using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class DiamondEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;
        private readonly DiamondData _diamondData;

        private ConfigEntity _statsEntity;

        public DiamondEnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
            _diamondData = data.DiamondData;
        }

        public void Initialize()
        {
            _statsEntity = _contexts.config.statsConfigEntity;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.Diamond);
        }


        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.hasTransform && entity.hasIndex;
        }


        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;
            for (var i = 0; i < entities.Count; i++)
            {
                var explosion = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS, _diamondData.ExplosionPrefab);
                explosion.position = new Vector3(entities[i].transform.Value.position.x,
                    entities[i].transform.Value.position.y, explosion.position.z);

                _poolService.Despawn(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS, entities[i].transform.Value);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_cameraService.Container);
                    eDestroy.AddTarget(explosion);
                    eDestroy.AddOffset(GameSettings.GetTopTransformPoint(5.12f));
                    eDestroy.AddPool(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS);
                }

                var costSize = _diamondData.DefaultCostSize;
                if (entities[i].index.Value != 0)
                    costSize = _diamondData.OtherPrefabList[entities[i].index.Value - 1].CostSize;
                _statsEntity.ReplaceStatsDiamond(_statsEntity.statsDiamond.Value + costSize);
            }
        }
    }
}