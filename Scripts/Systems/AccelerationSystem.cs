using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public abstract class AccelerationSystem<T> : IExecuteSystem where T : Entity
    {
        protected IGroup<T> _group;
        protected List<T> _buffer;

        public AccelerationSystem()
        {
            _buffer = new List<T>();
        }

        public virtual void Execute()
        {
            if (_group.count == 0) return;

            _group.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
                Execute(_buffer[i]);
        }

        protected virtual void Execute(T entity)
        {
        }
    }

    public class AccelerationFlightSystem : AccelerationSystem<FlightEntity>
    {
        public AccelerationFlightSystem(Contexts contexts) : base()
        {
            _group = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Speed,
                FlightMatcher.Acceleration));
        }

        protected override void Execute(FlightEntity entity)
        {
            entity.ReplaceSpeed(entity.speed.Value + entity.acceleration.Value * Time.deltaTime);
        }
    }

    public class AccelerationEnvironmentSystem : AccelerationSystem<EnvironmentEntity>
    {
        public AccelerationEnvironmentSystem(Contexts contexts) : base()
        {
            _group = contexts.environment.GetGroup(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.Speed,
                EnvironmentMatcher.Acceleration));
        }

        protected override void Execute(EnvironmentEntity entity)
        {
            entity.ReplaceSpeed(entity.speed.Value + entity.acceleration.Value * Time.deltaTime);
        }
    }
}