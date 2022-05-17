using Entitas;

namespace GameEngine
{
    public sealed class GuideOffsetFlightSystem : IExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly Contexts _contexts;

        public GuideOffsetFlightSystem(Contexts contexts, Services services)
        {
            _contexts = contexts;
            _timeService = services.TimeService;
        }

        public void Execute()
        {
            Execute(_contexts.flight.guideOffsetEntity);
        }

        private void Execute(FlightEntity e)
        {
            if (!e.isEnabled) return;

            if (e.direction.Value.x == -1)
            {
                e.offset.Value -= e.speed.Value * _timeService.DeltaTime;
                if (e.offset.Value <= -e.limit.Value)
                {
                    e.offset.Value = -e.limit.Value;
                    e.direction.Value.x = 1;
                }
            }
            else if (e.direction.Value.x == 1)
            {
                e.offset.Value += e.speed.Value * _timeService.DeltaTime;
                if (e.offset.Value >= e.limit.Value)
                {
                    e.offset.Value = e.limit.Value;
                    e.direction.Value.x = -1;
                }
            }
            else
            {
                return;
            }

            var position = e.transform.Value.position;
            position.x += e.offset.Value * _timeService.DeltaTime;
            e.transform.Value.position = position;
        }
    }
}