using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class ResurrectionAfterDeathFlightSystem : ReactiveSystem<FlightEntity>
    {
        private readonly Contexts _contexts;

        public ResurrectionAfterDeathFlightSystem(Contexts contexts) : base(contexts.flight)
        {
            _contexts = contexts;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.Dead);
        }


        protected override bool Filter(FlightEntity entity)
        {
            return true;
        }

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