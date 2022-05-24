using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Поворачивает объект в сторону заданной цели.
    /// </summary>
    public sealed class LookAtSystem : IExecuteSystem
    {
        private readonly IGroup<FlightEntity> _entities;
        private readonly List<FlightEntity> _buffer;

        public LookAtSystem(Contexts contexts)
        {
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
                    _buffer[i].speed.Value * Time.deltaTime);
            }
        }
    }
}