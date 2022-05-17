using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class PinFlightSystem : IExecuteSystem
    {
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public PinFlightSystem(Contexts contexts)
        {
            _entities = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Pin,
                FlightMatcher.Transform,
                FlightMatcher.Target));
            _buffer = new List<FlightEntity>();
        }


        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (!_buffer[i].hasDirection)
                {
                    _buffer[i].transform.Value.position = _buffer[i].target.Value.position;
                    continue;
                }

                var position = _buffer[i].transform.Value.position;
                if (_buffer[i].direction.Value.x != 0)
                    position.x = _buffer[i].target.Value.position.x;
                if (_buffer[i].direction.Value.y != 0)
                    position.y = _buffer[i].target.Value.position.y;
                if (_buffer[i].direction.Value.z != 0)
                    position.z = _buffer[i].target.Value.position.z;
                _buffer[i].transform.Value.position = position;
            }
        }
    }
}