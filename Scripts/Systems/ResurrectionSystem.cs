using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class ResurrectionSystem : ReactiveSystem<FlightEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly FlightSettings _flightSettings;
        private readonly IGroup<FlightEntity> _characterGroup;
        private readonly List<FlightEntity> _characterBuffer;
        private readonly Transform _camera;

        private ConfigEntity _mainConfigEntity;
        private FlightEntity _movementEntity;

        public ResurrectionSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.flight)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _flightSettings = settings.FlightSettings;
            _characterGroup = contexts.flight.GetGroup(FlightMatcher.Character);
            _characterBuffer = new List<FlightEntity>();
            _camera = camera;
        }

        public void Initialize()
        {
            if (_contexts.config.isMainConfig)
                _mainConfigEntity = _contexts.config.mainConfigEntity;
            _movementEntity = _contexts.flight.movementEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.Resurrection);

        protected override bool Filter(FlightEntity entity) => true;

        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;
            if (_mainConfigEntity.isGameOver) return;

            for (var i = 0; i < entities.Count; i++)
                if (!Execute(entities[i]))
                    break;
        }

        private bool Execute(FlightEntity e)
        {
            _characterGroup.GetEntities(_characterBuffer);

            {
                if (_mainConfigEntity.lifeCountInStorage.Value <= 0) return false;

                var flightCount = 0;
                for (var i = 0; i < _characterBuffer.Count; i++)
                {
                    if (_characterBuffer[i].hasDeath) continue;
                    flightCount++;
                }

                if (flightCount >= _flightSettings.MaxSizeInFlight) return false;
            }

            Transform target;
            {
                FlightEntity targetEntity = null;
                for (var i = 0; i < _characterBuffer.Count; i++)
                {
                    if (_characterBuffer[i].hasDeath) continue;
                    if (targetEntity == null || targetEntity.index.Value < _characterBuffer[i].index.Value)
                        targetEntity = _characterBuffer[i];
                }

                target = targetEntity != null
                    ? targetEntity.transform.Value
                    : _contexts.flight.guideOffsetEntity.transform.Value;
            }

            var character = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT, _flightSettings.CharacterPrefab);
            character.position = new Vector3(character.position.x, ScreenSettings.GetBottomScreenPoint(
                _camera.position.y - 5.12f), character.position.z);

            var followEntity = _contexts.flight.CreateEntity();
            {
                float maxSpeed = _flightSettings.FollowSpeed + _flightSettings.FollowSpeed / 100 *
                    (_flightSettings.LimitSpeed - _flightSettings.Speed) / (_flightSettings.Speed / 100);
                float speed = _flightSettings.FollowSpeed + _flightSettings.FollowSpeed / 100 *
                    (_movementEntity.maxSpeed.Value - _movementEntity.speed.Value) / _movementEntity.speed.Value * 100;

                followEntity.isFollow = true;
                followEntity.AddTransform(character);
                followEntity.AddTarget(target);
                followEntity.AddSpeed(speed);
                followEntity.AddAcceleration(_flightSettings.AccelerationSpeed);
                followEntity.AddMaxSpeed(maxSpeed);
                followEntity.AddDistance(new Vector3(0, _flightSettings.FollowDistance, 0));
            }

            var rotationEntity = _contexts.flight.CreateEntity();
            {
                rotationEntity.isRotation = true;
                rotationEntity.AddTransform(character);
                rotationEntity.AddTarget(target);
                rotationEntity.AddSpeed(_flightSettings.RotationSpeed);
            }

            var characterEntity = _contexts.flight.CreateEntity();
            {
                characterEntity.AddCharacter(followEntity, rotationEntity);
                characterEntity.AddIndex(_contexts.config.mainConfigEntity.flightIndex.Value);
                characterEntity.AddTransform(character);
                characterEntity.AddTarget(target);
            }

            _contexts.config.mainConfigEntity.ReplaceFlightIndex(
                _contexts.config.mainConfigEntity.flightIndex.Value + 1);
            _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                _contexts.config.mainConfigEntity.lifeCountInStorage.Value - 1);

            return true;
        }
    }
}