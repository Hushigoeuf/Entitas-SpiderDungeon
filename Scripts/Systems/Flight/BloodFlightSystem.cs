using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class BloodFlightSystem : ReactiveSystem<FlightEntity>
    {
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;
        private readonly BloodData _bloodData;

        private Dictionary<int, List<BloodPrefab>> _prefabList = new Dictionary<int, List<BloodPrefab>>();

        public BloodFlightSystem(Contexts contexts, Services services, Data data) : base(contexts.flight)
        {
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
            _bloodData = data.BloodData;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.Spider,
                FlightMatcher.Dead));
        }


        protected override bool Filter(FlightEntity entity)
        {
            return entity.hasDead;
        }


        protected override void Execute(List<FlightEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
                Execute(entities[i]);
        }

        private void Execute(FlightEntity eSpider)
        {
            if (!_prefabList.ContainsKey(eSpider.dead.ObstacleTypeIndex))
                _prefabList.Add(eSpider.dead.ObstacleTypeIndex, _bloodData.GetPrefabs(eSpider.dead.ObstacleTypeIndex));

            foreach (var prefab in _prefabList[eSpider.dead.ObstacleTypeIndex])
            {
                var blood = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT_BLOODS, prefab.Prefab);
                blood.transform.position =
                    new Vector3(prefab.IgnorePosition ? blood.transform.position.x : eSpider.transform.Value.position.x,
                        eSpider.transform.Value.position.y, blood.transform.position.z);
                if (!prefab.IgnoreRotation)
                    blood.eulerAngles = new Vector3(blood.eulerAngles.x, blood.eulerAngles.y,
                        eSpider.transform.Value.eulerAngles.z + prefab.OffsetRotation);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_cameraService.Container);
                    eDestroy.AddTarget(blood);
                    eDestroy.AddOffset(GameSettings.GetTopTransformPoint(5.12f));
                    eDestroy.AddPool(GameSettings.POOL_ID_FLIGHT_BLOODS);
                }
            }
        }
    }
}