using Entitas;

namespace GameEngine
{
    public sealed class EnvironmentPositionFlightSystem : IInitializeSystem, IExecuteSystem
    {
        private const float CELL_SIZE = 3.5f;
        private const float CELL_HALL_SIZE = .5f;

        private readonly Contexts _contexts;

        private bool _enabled;
        private FlightEntity _guideEntity;

        public EnvironmentPositionFlightSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void Initialize()
        {
            if (_contexts.flight.isGuide)
            {
                _guideEntity = _contexts.flight.guideEntity;
                if (!_guideEntity.hasEnvironmentPosition)
                    _guideEntity.AddEnvironmentPosition(0, 0, 0, 0);
            }

            _enabled = _guideEntity != null;
        }

        public void Execute()
        {
            if (!_enabled) return;

            var position = _guideEntity.transform.Value.position.x;

            _guideEntity.environmentPosition.Set(0);

            // X1
            if (position >= -CELL_SIZE * 2 && position <= -CELL_SIZE + CELL_HALL_SIZE)
            {
                _guideEntity.environmentPosition.X0++;
                if (position >= -CELL_SIZE * 2 + CELL_HALL_SIZE && position <= -CELL_SIZE - CELL_HALL_SIZE)
                    _guideEntity.environmentPosition.X0++;
            }

            // X2
            if (position >= -CELL_SIZE - CELL_HALL_SIZE && position <= 0 + CELL_HALL_SIZE)
            {
                _guideEntity.environmentPosition.X1++;
                if (position >= -CELL_SIZE + CELL_HALL_SIZE && position <= -CELL_HALL_SIZE)
                    _guideEntity.environmentPosition.X1++;
            }

            // X3
            if (position >= 0 - CELL_HALL_SIZE && position <= CELL_SIZE + CELL_HALL_SIZE)
            {
                _guideEntity.environmentPosition.X2++;
                if (position >= CELL_HALL_SIZE && position <= CELL_SIZE - CELL_HALL_SIZE)
                    _guideEntity.environmentPosition.X2++;
            }

            // X4
            if (position >= CELL_SIZE - CELL_HALL_SIZE && position <= CELL_SIZE * 2)
            {
                _guideEntity.environmentPosition.X3++;
                if (position >= CELL_SIZE + CELL_HALL_SIZE && position <= CELL_SIZE * 2 - CELL_HALL_SIZE)
                    _guideEntity.environmentPosition.X3++;
            }
        }
    }
}