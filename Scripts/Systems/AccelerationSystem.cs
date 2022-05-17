using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public abstract class AccelerationSystem<T> : IExecuteSystem where T : Entity
    {
        protected readonly ITimeService _timeService;
        protected IGroup<T> _group;
        protected List<T> _buffer;

        public AccelerationSystem(Contexts contexts, Services services)
        {
            _timeService = services.TimeService;
            _buffer = new List<T>();
        }

        public void Execute()
        {
            if (_group.count == 0) return;
            _group.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++) Execute(_buffer[i]);
        }

        protected virtual void Execute(T entity)
        {
        }
    }

    public sealed class AccelerationFlightSystem : AccelerationSystem<FlightEntity>
    {
        public AccelerationFlightSystem(Contexts contexts, Services services) : base(contexts, services)
        {
            _group = contexts.flight.GetGroup(FlightMatcher.AllOf(
                FlightMatcher.Speed,
                FlightMatcher.Acceleration));
        }

        protected override void Execute(FlightEntity entity)
        {
            entity.ReplaceSpeed(entity.speed.Value + entity.acceleration.Value * _timeService.DeltaTime);
        }
    }

    public sealed class AccelerationEnvironmentSystem : AccelerationSystem<EnvironmentEntity>
    {
        public AccelerationEnvironmentSystem(Contexts contexts, Services services) : base(contexts, services)
        {
            _group = contexts.environment.GetGroup(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.Speed,
                EnvironmentMatcher.Acceleration));
        }

        protected override void Execute(EnvironmentEntity entity)
        {
            entity.ReplaceSpeed(entity.speed.Value + entity.acceleration.Value * _timeService.DeltaTime);
        }
    }
}