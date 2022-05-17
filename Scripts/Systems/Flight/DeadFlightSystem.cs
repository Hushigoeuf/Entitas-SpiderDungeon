using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class DeadFlightSystem : ReactiveSystem<FlightEntity>, IInitializeSystem, ICleanupSystem
    {
        private readonly Contexts _contexts;
        private readonly IGroup<FlightEntity> _spiderGroup;
        private readonly IPoolService _poolService;
        //private readonly FlightData _flightData;

        private readonly List<FlightEntity> _entityList = new List<FlightEntity>();
        private readonly List<FlightEntity> _buffer = new List<FlightEntity>();

        private ConfigEntity _statsEntity;
        //private FlightEntity _guideEntity;

        public DeadFlightSystem(Contexts contexts, Services services, Data data) : base(contexts.flight)
        {
            _contexts = contexts;
            _poolService = services.PoolService;
            _spiderGroup = contexts.flight.GetGroup(FlightMatcher.Spider);
            //_flightData = data.FlightData;
        }

        public void Initialize()
        {
            _statsEntity = _contexts.config.statsConfigEntity;
            //_guideEntity = _contexts.flight.movementEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.Spider,
                FlightMatcher.Dead));
        }


        protected override bool Filter(FlightEntity entity)
        {
            return true;
        }


        protected override void Execute(List<FlightEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
                Execute(entities[i]);
        }

        private void Execute(FlightEntity eSpider)
        {
            var entities = _spiderGroup.GetEntities(_buffer);
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i].hasDead) continue;
                _entityList.Add(entities[i]);
            }

            _entityList.Sort((e0, e1) =>
            {
                return e0.index.Value < e1.index.Value ? -1 : e0.index.Value > e1.index.Value ? 1 : 0;
            });

            Transform target = null;
            for (var i = 0; i < _entityList.Count; i++)
            {
                if (target == null)
                    target = _contexts.flight.guideOffsetEntity.transform.Value;

                _entityList[i].spider.FollowEntity.ReplaceTarget(target);
                _entityList[i].spider.RotationEntity.ReplaceTarget(target);
                _entityList[i].ReplaceTarget(target);

                target = _entityList[i].transform.Value;
            }

            /*if (_flightData.ExtraSpeedPerSpider > 0)
                _guideEntity.ReplaceSpeed(_flightData.Speed +
                                          _flightData.MaxSizeInFlight * _flightData.ExtraSpeedPerSpider -
                                          _flightData.ExtraSpeedPerSpider * _entityList.Count);*/

            _entityList.Clear();
        }

        public void Cleanup()
        {
            foreach (var e in _spiderGroup.GetEntities(_buffer))
            {
                if (!e.hasDead) continue;

                if (e.hasTrapType) _statsEntity.statsDead.CategoryCountGroupList[e.trapType.InstanceId]++;
                else _statsEntity.statsDead.NoCategoryCount++;

                e.spider.FollowEntity.Destroy();
                e.spider.RotationEntity.Destroy();

                _poolService.Despawn(GameSettings.POOL_ID_FLIGHT, e.transform.Value);

                e.Destroy();
            }
        }
    }
}