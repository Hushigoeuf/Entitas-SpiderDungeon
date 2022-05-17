using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class ResurrectionFlightSystem : ReactiveSystem<FlightEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;
        private readonly FlightData _flightData;

        //private readonly IGroup<FlightEntity> _resurrectionGroup;
        private readonly IGroup<FlightEntity> _spiderGroup;

        //private readonly List<FlightEntity> _resurrectionBuffer;
        private readonly List<FlightEntity> _spiderBuffer;

        private ConfigEntity _mainConfigEntity;
        private FlightEntity _movementEntity;

        public ResurrectionFlightSystem(Contexts contexts, Services services, Data data) : base(contexts.flight)
        {
            _contexts = contexts;
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
            _flightData = data.FlightData;

            //_resurrectionGroup = contexts.flight.GetGroup(FlightMatcher.Resurrection);
            _spiderGroup = contexts.flight.GetGroup(FlightMatcher.Spider);
            //_resurrectionBuffer = new List<FlightEntity>();
            _spiderBuffer = new List<FlightEntity>();
        }

        public void Initialize()
        {
            if (_contexts.config.isMainConfig)
                _mainConfigEntity = _contexts.config.mainConfigEntity;
            _movementEntity = _contexts.flight.movementEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.Resurrection);
        }


        protected override bool Filter(FlightEntity entity)
        {
            return true;
        }


        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;
            //_spiderGroup.GetEntities(_spiderBuffer);
            //_resurrectionGroup.GetEntities(_resurrectionBuffer);

            if (_mainConfigEntity.isGameOver) return;

            for (var i = 0; i < entities.Count; i++)
                if (!Execute(entities[i]))
                    break;
        }

        private bool Execute(FlightEntity e)
        {
            _spiderGroup.GetEntities(_spiderBuffer);

            {
                if (_mainConfigEntity.lifeCountInStorage.Value <= 0) return false;

                var flightCount = 0;
                for (var i = 0; i < _spiderBuffer.Count; i++)
                {
                    if (_spiderBuffer[i].hasDead) continue;
                    flightCount++;
                }

                if (flightCount >= _flightData.MaxSizeInFlight) return false;
            }

            Transform target;
            {
                FlightEntity targetEntity = null;
                for (var i = 0; i < _spiderBuffer.Count; i++)
                {
                    if (_spiderBuffer[i].hasDead) continue;
                    if (targetEntity == null || targetEntity.index.Value < _spiderBuffer[i].index.Value)
                        targetEntity = _spiderBuffer[i];
                }

                target = targetEntity != null
                    ? targetEntity.transform.Value
                    : _contexts.flight.guideOffsetEntity.transform.Value;
            }

            var spider = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightData.SpiderPrefab);
            spider.position = new Vector3(
                spider.position.x,
                GameSettings.GetDownTransformPoint(_cameraService.Container.position.y - 5.12f),
                spider.position.z);

            var eSpiderFollow = _contexts.flight.CreateEntity();
            {
                var maxSpeed = _flightData.FollowSpeed + _flightData.FollowSpeed / 100 *
                    (_flightData.MaxSpeed - _flightData.Speed) / (_flightData.Speed / 100);
                var speed = _flightData.FollowSpeed + _flightData.FollowSpeed / 100 *
                    (_movementEntity.maxSpeed.Value - _movementEntity.speed.Value) / _movementEntity.speed.Value * 100;

                eSpiderFollow.isFollow = true;
                eSpiderFollow.AddTransform(spider);
                eSpiderFollow.AddTarget(target);
                eSpiderFollow.AddSpeed(speed);
                eSpiderFollow.AddAcceleration(_flightData.AccelerationSpeed);
                eSpiderFollow.AddMaxSpeed(maxSpeed);
                eSpiderFollow.AddDistance(new Vector3(0, _flightData.FollowDistance, 0));
            }

            var eSpiderRotation = _contexts.flight.CreateEntity();
            {
                eSpiderRotation.isRotation = true;
                eSpiderRotation.AddTransform(spider);
                eSpiderRotation.AddTarget(target);
                eSpiderRotation.AddSpeed(_flightData.RotationSpeed);
            }

            var eSpider = _contexts.flight.CreateEntity();
            {
                eSpider.AddSpider(eSpiderFollow, eSpiderRotation);
                eSpider.AddIndex(_contexts.config.mainConfigEntity.flightIndex.Value);
                eSpider.AddTransform(spider);
                eSpider.AddTarget(target);
            }

            _contexts.config.mainConfigEntity.ReplaceFlightIndex(
                _contexts.config.mainConfigEntity.flightIndex.Value + 1);
            _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                _contexts.config.mainConfigEntity.lifeCountInStorage.Value - 1);

            return true;
        }
    }
}