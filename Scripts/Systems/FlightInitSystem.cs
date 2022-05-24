using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Создает персонажей и инициализирует соответствующие сущности.
    /// </summary>
    public sealed class FlightInitSystem : IInitializeSystem
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
            // Создает первичного персонажа (светлячок)
            var guide = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightSettings.GuidePrefab);
            guide.position = new Vector3(0, 0, 0);

            var characters = new Transform[_contexts.config.mainConfigEntity.minFlightSize.Value];
            for (var i = 0; i < characters.Length; i++)
            {
                // Создает дочернего персонажа (паук)
                characters[i] = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightSettings.CharacterPrefab);
                characters[i].position = new Vector3(0, -5.12f * (i + 1), 0);

                // Создает сущность, чтобы персонаж следовал за заданной целью
                var followEntity = _contexts.flight.CreateEntity();
                {
                    var maxSpeed = _flightSettings.FollowSpeed + _flightSettings.FollowSpeed / 100 *
                        (_flightSettings.LimitSpeed - _flightSettings.Speed) / (_flightSettings.Speed / 100);

                    followEntity.isFollow = true;
                    followEntity.AddTransform(characters[i]);
                    followEntity.AddTarget(i == 0 ? guide : characters[i - 1]);
                    followEntity.AddSpeed(_flightSettings.FollowSpeed);
                    followEntity.AddAcceleration(_flightSettings.AccelerationSpeed);
                    followEntity.AddMaxSpeed(maxSpeed);
                    followEntity.AddDistance(new Vector3(0, _flightSettings.FollowDistance, 0));
                }

                // Создает сущность, чтобы вращать персонаж в сторону заданной цели
                var rotationEntity = _contexts.flight.CreateEntity();
                {
                    rotationEntity.isRotation = true;
                    rotationEntity.AddTransform(characters[i]);
                    rotationEntity.AddTarget(i == 0 ? guide : characters[i - 1]);
                    rotationEntity.AddSpeed(_flightSettings.RotationSpeed);
                }

                // Создает основную сущность дочернего персонажа, которая хранит информацию о нем
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

            // Создает сущность, чтобы первичный персонаж двигался в заданном направлении
            var movementEntity = _contexts.flight.CreateEntity();
            {
                movementEntity.isMovement = true;
                movementEntity.AddTransform(guide);
                movementEntity.AddSpeed(_flightSettings.Speed);
                movementEntity.AddAcceleration(_flightSettings.AccelerationSpeed);
                movementEntity.AddMaxSpeed(_flightSettings.LimitSpeed);
                movementEntity.AddDirection(new Vector3(0, 1, 0));
            }

            // Создает сущность, чтобы первичный персонаж двигался из стороны в сторону
            var offsetEntity = _contexts.flight.CreateEntity();
            {
                offsetEntity.isGuideOffset = true;
                offsetEntity.isEnabled = true;
                offsetEntity.AddOffset(0);
                offsetEntity.AddTransform(guide);
                offsetEntity.AddSpeed(_flightSettings.OffsetSpeed);
                offsetEntity.AddLimit(_flightSettings.OffsetLimit);
                offsetEntity.AddDirection(new Vector3(_randomService.Choose(-1, 1, -1, 1), 0, 0));
            }

            // Создает сущность, которая "привязывает" камеру к заданной цели
            var pinEntity = _contexts.flight.CreateEntity();
            {
                pinEntity.isPin = true;
                pinEntity.AddTransform(_camera);
                pinEntity.AddTarget(guide);
                pinEntity.AddDirection(new Vector3(0, 1, 0));
            }

            // Создает основную сущность первичного персонажа, которая хранит информацию о нем
            var guideEntity = _contexts.flight.CreateEntity();
            {
                guideEntity.isGuide = true;
                guideEntity.AddTransform(guide);
            }
        }
    }
}