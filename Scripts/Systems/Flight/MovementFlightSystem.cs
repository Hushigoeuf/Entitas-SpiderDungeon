using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class MovementFlightSystem : IExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public MovementFlightSystem(Contexts contexts, Services services)
        {
            _timeService = services.TimeService;
            _entities = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Movement,
                FlightMatcher.Transform,
                FlightMatcher.Speed,
                FlightMatcher.Direction));
            _buffer = new List<FlightEntity>();
        }


        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
                _buffer[i].transform.Value.position += _buffer[i].direction.Value * _buffer[i].speed.Value * _timeService.DeltaTime;
        }
    }
}