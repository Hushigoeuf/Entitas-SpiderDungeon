using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class DeathStatusSystem : ReactiveSystem<FlightEntity>
    {
        private readonly IGroup<FlightEntity> _characterGroup;
        private readonly List<FlightEntity> _buffer = new List<FlightEntity>();

        public DeathStatusSystem(Contexts contexts) : base(contexts.flight)
        {
            _characterGroup = contexts.flight.GetGroup(FlightMatcher.Character);
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.DeathStatus,
                FlightMatcher.Transform));

        protected override bool Filter(FlightEntity entity) => true;

        protected override void Execute(List<FlightEntity> entities)
        {
            foreach (var deathEntity in entities)
            foreach (var characterEntity in _characterGroup.GetEntities(_buffer))
            {
                if (deathEntity.transform.Value != characterEntity.transform.Value) continue;
                if (!characterEntity.hasDeath) characterEntity.AddDeath(deathEntity.index.Value);
                else characterEntity.ReplaceDeath(deathEntity.index.Value);
                if (deathEntity.hasTrapType)
                {
                    if (!characterEntity.hasTrapType) characterEntity.AddTrapType(deathEntity.trapType.InstanceID);
                    else characterEntity.ReplaceTrapType(deathEntity.trapType.InstanceID);
                }

                break;
            }
        }
    }
}