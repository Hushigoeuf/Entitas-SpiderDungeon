using Entitas;

namespace GameEngine
{
    public sealed class InputOffsetFlightSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly ITimeService _timeService;

        public InputOffsetFlightSystem(Contexts contexts, Services services)
        {
            _contexts = contexts;
            _timeService = services.TimeService;
        }

        public void Execute()
        {
            var eLeftPointer = _contexts.flight.leftPointerEntity;
            var eRightPointer = _contexts.flight.rightPointerEntity;
            var eGuideOffset = _contexts.flight.guideOffsetEntity;

            if (!eLeftPointer.isEnabled && !eRightPointer.isEnabled) return;

            var position = eGuideOffset.transform.Value.position;
            if (eLeftPointer.isEnabled)
                position.x -= eLeftPointer.offset.Value * _timeService.DeltaTime;
            else if (eRightPointer.isEnabled) position.x += eRightPointer.offset.Value * _timeService.DeltaTime;
            eGuideOffset.transform.Value.position = position;
        }
    }
}