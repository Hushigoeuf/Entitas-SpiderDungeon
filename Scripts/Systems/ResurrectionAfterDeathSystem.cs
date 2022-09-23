using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class ResurrectionAfterDeathSystem : ReactiveSystem<FlightEntity>
    {
        private readonly Contexts _contexts;

        public ResurrectionAfterDeathSystem(Contexts contexts) : base(contexts.flight)
        {
            _contexts = contexts;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.Death);

        protected override bool Filter(FlightEntity entity) => true;

        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;
            for (var i = 0; i < entities.Count; i++)
            {
                var e = _contexts.flight.CreateEntity();
                {
                    e.isResurrection = true;
                }
            }
        }
    }
}