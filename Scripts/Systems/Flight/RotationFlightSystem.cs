using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class RotationFlightSystem : IExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public RotationFlightSystem(Contexts contexts, Services services)
        {
            _timeService = services.TimeService;
            _entities = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Rotation,
                FlightMatcher.Transform,
                FlightMatcher.Target,
                FlightMatcher.Speed));
            _buffer = new List<FlightEntity>();
        }


        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                var direction = _buffer[i].target.Value.position - _buffer[i].transform.Value.position;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                _buffer[i].transform.Value.rotation = Quaternion.Slerp(
                    _buffer[i].transform.Value.rotation, rotation,
                    _buffer[i].speed.Value * _timeService.DeltaTime);
            }
        }
    }
}