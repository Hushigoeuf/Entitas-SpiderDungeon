using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class DeadStatusFlightSystem : ReactiveSystem<FlightEntity>
    {
        private readonly IGroup<FlightEntity> _spiderGroup;
        private readonly List<FlightEntity> _buffer = new List<FlightEntity>();

        public DeadStatusFlightSystem(Contexts contexts) : base(contexts.flight)
        {
            _spiderGroup = contexts.flight.GetGroup(FlightMatcher.Spider);
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.DeadStatus,
                FlightMatcher.Transform));
        }


        protected override bool Filter(FlightEntity entity)
        {
            return true;
        }


        protected override void Execute(List<FlightEntity> entities)
        {
            foreach (var eDead in entities)
            foreach (var eSpider in _spiderGroup.GetEntities(_buffer))
            {
                if (eDead.transform.Value != eSpider.transform.Value) continue;
                if (!eSpider.hasDead) eSpider.AddDead(eDead.index.Value);
                else eSpider.ReplaceDead(eDead.index.Value);
                if (eDead.hasTrapType)
                {
                    if (!eSpider.hasTrapType) eSpider.AddTrapType(eDead.trapType.InstanceId);
                    else eSpider.ReplaceTrapType(eDead.trapType.InstanceId);
                }

                break;
            }
        }
    }
}