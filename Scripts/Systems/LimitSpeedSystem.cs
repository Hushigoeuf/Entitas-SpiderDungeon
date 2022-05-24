using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Ограничивает скорость минимальным и максимальным значениями.
    /// </summary>
    public class LimitSpeedFlightSystem : ReactiveSystem<FlightEntity>
    {
        public LimitSpeedFlightSystem(Contexts contexts) : base(contexts.flight)
        {
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.AllOf(FlightMatcher.Speed)
                .AnyOf(FlightMatcher.MinSpeed, FlightMatcher.MaxSpeed));

        protected override bool Filter(FlightEntity entity) =>
            entity.hasMinSpeed && entity.speed.Value < entity.minSpeed.Value
            || entity.hasMaxSpeed && entity.speed.Value > entity.maxSpeed.Value;

        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i].hasMinSpeed && entities[i].speed.Value < entities[i].minSpeed.Value)
                    entities[i].ReplaceSpeed(entities[i].minSpeed.Value);
                if (entities[i].hasMaxSpeed && entities[i].speed.Value > entities[i].maxSpeed.Value)
                    entities[i].ReplaceSpeed(entities[i].maxSpeed.Value);
            }
        }
    }

    public class LimitSpeedEnvironmentSystem : ReactiveSystem<EnvironmentEntity>
    {
        public LimitSpeedEnvironmentSystem(Contexts contexts) : base(contexts.environment)
        {
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(EnvironmentMatcher.Speed)
                .AnyOf(EnvironmentMatcher.MinSpeed, EnvironmentMatcher.MaxSpeed));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.hasMinSpeed && entity.speed.Value < entity.minSpeed.Value
            || entity.hasMaxSpeed && entity.speed.Value > entity.maxSpeed.Value;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i].hasMinSpeed && entities[i].speed.Value < entities[i].minSpeed.Value)
                    entities[i].ReplaceSpeed(entities[i].minSpeed.Value);
                if (entities[i].hasMaxSpeed && entities[i].speed.Value > entities[i].maxSpeed.Value)
                    entities[i].ReplaceSpeed(entities[i].maxSpeed.Value);
            }
        }
    }
}