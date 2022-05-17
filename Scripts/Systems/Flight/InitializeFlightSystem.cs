using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class InitializeFlightSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly IRandomService _randomService;
        private readonly ICameraService _cameraService;
        private readonly FlightData _flightData;

        public InitializeFlightSystem(Contexts contexts, Services services, Data data)
        {
            _contexts = contexts;
            _poolService = services.PoolService;
            _randomService = services.RandomService;
            _cameraService = services.CameraService;
            _flightData = data.FlightData;
        }

        public void Initialize()
        {
            var guide = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightData.GuidePrefab);
            guide.position = new Vector3(0, 0, 0);

            var spiders = new Transform[_contexts.config.mainConfigEntity.minFlightSize.Value];
            for (var i = 0; i < spiders.Length; i++)
            {
                spiders[i] = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightData.SpiderPrefab);
                spiders[i].position = new Vector3(0, -5.12f * (i + 1), 0);

                var eSpiderFollow = _contexts.flight.CreateEntity();
                {
                    var maxSpeed = _flightData.FollowSpeed + _flightData.FollowSpeed / 100 *
                        (_flightData.MaxSpeed - _flightData.Speed) / (_flightData.Speed / 100);

                    eSpiderFollow.isFollow = true;
                    eSpiderFollow.AddTransform(spiders[i]);
                    eSpiderFollow.AddTarget(i == 0 ? guide : spiders[i - 1]);
                    eSpiderFollow.AddSpeed(_flightData.FollowSpeed);
                    eSpiderFollow.AddAcceleration(_flightData.AccelerationSpeed);
                    eSpiderFollow.AddMaxSpeed(maxSpeed);
                    eSpiderFollow.AddDistance(new Vector3(0, _flightData.FollowDistance, 0));
                }

                var eSpiderRotation = _contexts.flight.CreateEntity();
                {
                    eSpiderRotation.isRotation = true;
                    eSpiderRotation.AddTransform(spiders[i]);
                    eSpiderRotation.AddTarget(i == 0 ? guide : spiders[i - 1]);
                    eSpiderRotation.AddSpeed(_flightData.RotationSpeed);
                }

                var eSpider = _contexts.flight.CreateEntity();
                {
                    eSpider.AddSpider(eSpiderFollow, eSpiderRotation);
                    eSpider.AddIndex(i);
                    eSpider.AddTransform(spiders[i]);
                    eSpider.AddTarget(i == 0 ? guide : spiders[i - 1]);
                }

                _contexts.config.mainConfigEntity.ReplaceFlightIndex(
                    _contexts.config.mainConfigEntity.flightIndex.Value + 1);
            }

            var eGuideMovement = _contexts.flight.CreateEntity();
            {
                eGuideMovement.isMovement = true;
                eGuideMovement.AddTransform(guide);
                eGuideMovement.AddSpeed(_flightData.Speed);
                eGuideMovement.AddAcceleration(_flightData.AccelerationSpeed);
                eGuideMovement.AddMaxSpeed(_flightData.MaxSpeed);
                eGuideMovement.AddDirection(new Vector3(0, 1, 0));
            }

            var eGuideOffset = _contexts.flight.CreateEntity();
            {
                eGuideOffset.isGuideOffset = true;
                eGuideOffset.isEnabled = true;
                eGuideOffset.AddOffset(0);
                eGuideOffset.AddTransform(guide);
                eGuideOffset.AddSpeed(_flightData.OffsetSpeed);
                eGuideOffset.AddLimit(_flightData.OffsetLimit);
                eGuideOffset.AddDirection(new Vector3(_randomService.Choose(-1, 1, -1, 1), 0, 0));
            }

            var eCameraPin = _contexts.flight.CreateEntity();
            {
                eCameraPin.isPin = true;
                eCameraPin.AddTransform(_cameraService.Container);
                eCameraPin.AddTarget(guide);
                eCameraPin.AddDirection(new Vector3(0, 1, 0));
            }

            var eGuide = _contexts.flight.CreateEntity();
            {
                eGuide.isGuide = true;
                eGuide.AddTransform(guide);
            }
        }
    }
}