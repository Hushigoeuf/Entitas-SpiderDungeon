using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class FlightInitSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly IRandomService _randomService;
        private readonly FlightSettings _flightSettings;
        private readonly Transform _camera;

        public FlightInitSystem(Contexts contexts, Services services, Settings settings, Transform camera)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _randomService = services.Random;
            _flightSettings = settings.FlightSettings;
            _camera = camera;
        }

        public void Initialize()
        {
            var guide = CreateGuideAndEntity();
            CreateMovementEntity(guide);
            CreateGuideOffsetEntity(guide);
            CreatePinEntity(guide);

            var characters = new Transform[_contexts.config.mainConfigEntity.minFlightSize.Value];
            for (var i = 0; i < characters.Length; i++)
            {
                characters[i] = CreateCharacter(i);

                var followEntity = CreateCharacterFollowEntity(characters[i], i == 0 ? guide : characters[i - 1]);
                var rotationEntity = CreateCharacterRotationEntity(characters[i], i == 0 ? guide : characters[i - 1]);

                var characterEntity = _contexts.flight.CreateEntity();
                {
                    characterEntity.AddCharacter(followEntity, rotationEntity);
                    characterEntity.AddIndex(i);
                    characterEntity.AddTransform(characters[i]);
                    characterEntity.AddTarget(i == 0 ? guide : characters[i - 1]);
                }

                _contexts.config.mainConfigEntity.ReplaceFlightIndex(
                    _contexts.config.mainConfigEntity.flightIndex.Value + 1);
            }
        }

        private Transform CreateGuideAndEntity()
        {
            var guide = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightSettings.GuidePrefab);
            guide.position = new Vector3(0, 0, 0);

            var guideEntity = _contexts.flight.CreateEntity();
            {
                guideEntity.isGuide = true;
                guideEntity.AddTransform(guide);
            }

            return guide;
        }

        private void CreateMovementEntity(Transform guide)
        {
            var movementEntity = _contexts.flight.CreateEntity();

            movementEntity.isMovement = true;
            movementEntity.AddTransform(guide);
            movementEntity.AddSpeed(_flightSettings.Speed);
            movementEntity.AddAcceleration(_flightSettings.AccelerationSpeed);
            movementEntity.AddMaxSpeed(_flightSettings.LimitSpeed);
            movementEntity.AddDirection(new Vector3(0, 1, 0));
        }

        private void CreateGuideOffsetEntity(Transform guide)
        {
            var offsetEntity = _contexts.flight.CreateEntity();

            offsetEntity.isGuideOffset = true;
            offsetEntity.isEnabled = true;
            offsetEntity.AddOffset(0);
            offsetEntity.AddTransform(guide);
            offsetEntity.AddSpeed(_flightSettings.OffsetSpeed);
            offsetEntity.AddLimit(_flightSettings.OffsetLimit);
            offsetEntity.AddDirection(new Vector3(_randomService.Choose(-1, 1, -1, 1), 0, 0));
        }

        private void CreatePinEntity(Transform guide)
        {
            var pinEntity = _contexts.flight.CreateEntity();

            pinEntity.isPin = true;
            pinEntity.AddTransform(_camera);
            pinEntity.AddTarget(guide);
            pinEntity.AddDirection(new Vector3(0, 1, 0));
        }

        private Transform CreateCharacter(int index)
        {
            var character = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightSettings.CharacterPrefab);
            character.position = new Vector3(0, -5.12f * (index + 1), 0);

            return character;
        }

        private FlightEntity CreateCharacterFollowEntity(Transform self, Transform target)
        {
            var followEntity = _contexts.flight.CreateEntity();

            float maxSpeed = _flightSettings.FollowSpeed + _flightSettings.FollowSpeed / 100 *
                (_flightSettings.LimitSpeed - _flightSettings.Speed) / (_flightSettings.Speed / 100);

            followEntity.isFollow = true;
            followEntity.AddTransform(self);
            followEntity.AddTarget(target);
            followEntity.AddSpeed(_flightSettings.FollowSpeed);
            followEntity.AddAcceleration(_flightSettings.AccelerationSpeed);
            followEntity.AddMaxSpeed(maxSpeed);
            followEntity.AddDistance(new Vector3(0, _flightSettings.FollowDistance, 0));

            return followEntity;
        }

        private FlightEntity CreateCharacterRotationEntity(Transform self, Transform target)
        {
            var rotationEntity = _contexts.flight.CreateEntity();

            rotationEntity.isRotation = true;
            rotationEntity.AddTransform(self);
            rotationEntity.AddTarget(target);
            rotationEntity.AddSpeed(_flightSettings.RotationSpeed);

            return rotationEntity;
        }
    }
}