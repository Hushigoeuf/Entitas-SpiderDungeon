using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Двигает объект в сторону заданной цели (преследование).
    /// </summary>
    public sealed class FollowSystem : IExecuteSystem
    {
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public FollowSystem(Contexts contexts)
        {
            _entities = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Follow,
                FlightMatcher.Transform,
                FlightMatcher.Target,
                FlightMatcher.Speed,
                FlightMatcher.Distance));
            _buffer = new List<FlightEntity>();
        }

        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                var position = _buffer[i].transform.Value.position;
                var resultPosition = Vector3.Lerp(_buffer[i].transform.Value.position,
                    _buffer[i].target.Value.position - _buffer[i].distance.Value,
                    _buffer[i].speed.Value * Time.deltaTime);
                position.x = resultPosition.x;
                position.y = resultPosition.y;
                _buffer[i].transform.Value.position = position;
            }
        }
    }
}