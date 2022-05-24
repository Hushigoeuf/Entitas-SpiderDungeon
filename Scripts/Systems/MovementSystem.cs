using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Двигает персонажей в заданном направлении.
    /// </summary>
    public sealed class MovementSystem : IExecuteSystem
    {
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public MovementSystem(Contexts contexts)
        {
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
                _buffer[i].transform.Value.position +=
                    _buffer[i].direction.Value * _buffer[i].speed.Value * Time.deltaTime;
        }
    }
}