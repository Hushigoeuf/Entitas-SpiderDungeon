using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class FollowSystem : IExecuteSystem
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
                _buffer[i].transform.Value.position = GetPosition(_buffer[i]);
        }

        private Vector3 GetPosition(FlightEntity flightEntity)
        {
            var position = flightEntity.transform.Value.position;
            var resultPosition = Vector3.Lerp(flightEntity.transform.Value.position,
                flightEntity.target.Value.position - flightEntity.distance.Value,
                flightEntity.speed.Value * Time.deltaTime);
            position.x = resultPosition.x;
            position.y = resultPosition.y;
            return position;
        }
    }
}