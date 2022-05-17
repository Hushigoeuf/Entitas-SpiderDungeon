using Entitas;

namespace GameEngine
{
    public sealed class UpdateInputOffsetFlightSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly ITimeService _timeService;
        private readonly FlightData _data;

        public UpdateInputOffsetFlightSystem(Contexts contexts, Services services, FlightData data)
        {
            _contexts = contexts;
            _timeService = services.TimeService;
            _data = data;
        }

        public void Execute()
        {
            Execute(_contexts.flight.leftPointerEntity);
            Execute(_contexts.flight.rightPointerEntity);
        }

        private void Execute(FlightEntity e)
        {
            if (e.isEnabled)
            {
                e.ReplaceOffset(e.offset.Value + e.speed.Value * _timeService.DeltaTime);
                if (e.offset.Value > e.limit.Value) e.ReplaceOffset(e.limit.Value);
            }
            else
            {
                e.ReplaceOffset(_data.InputStartOffset);
            }
        }
    }
}