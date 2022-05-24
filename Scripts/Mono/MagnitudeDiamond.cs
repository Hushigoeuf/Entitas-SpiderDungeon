using Entitas;
using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(MagnitudeDiamond))]
    public sealed class MagnitudeDiamond : GameEntitasBehaviour
    {
        public Transform CustomTarget;

        private EnvironmentEntity _magnitudeEntity;

        public void Colliding(MagnitudeDiamondActivated activated,
            GameObject collidingObject, ItemSettings itemSettings)
        {
            if (activated == null) return;
            if (collidingObject == null) return;
            if (itemSettings == null) return;

            if (!IsMainConfig) return;
            if (IsGameOver) return;
            if (_magnitudeEntity != null) return;

            var currentSpeed = _contexts.flight.movementEntity.speed.Value;

            _magnitudeEntity = _contexts.environment.CreateEntity();
            {
                _magnitudeEntity.isMagnitudeDiamond = true;
                _magnitudeEntity.AddTransform(CustomTarget ? CustomTarget : transform);
                _magnitudeEntity.AddTarget(_contexts.flight.guideEntity.transform.Value);
                _magnitudeEntity.AddSpeed(currentSpeed * itemSettings.MagnitudeStartSpeed);
                _magnitudeEntity.AddMinSpeed(currentSpeed * itemSettings.MagnitudeStartSpeed);
                _magnitudeEntity.AddAcceleration(currentSpeed * itemSettings.MagnitudeAccelerationSpeed);
                _magnitudeEntity.AddMaxSpeed(currentSpeed * itemSettings.MagnitudeMaxSpeed);
            }

            _magnitudeEntity.OnDestroyEntity += OnDestroyEntity;
        }

        private void OnDestroyEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyEntity;

            _magnitudeEntity = null;
        }

        protected override void OnGameOver()
        {
            base.OnGameOver();

            if (_magnitudeEntity != null)
                _magnitudeEntity.isCleanup = true;
            _magnitudeEntity = null;
        }

        private void OnDisable()
        {
            if (_magnitudeEntity != null)
                _magnitudeEntity.isCleanup = true;
            _magnitudeEntity = null;
        }
    }
}