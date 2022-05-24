using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Переключает автоматическое управление на ручное и обратно.
    /// </summary>
    public sealed class InputSwitchSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;

        public InputSwitchSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void Execute()
        {
            var eLeftPointer = _contexts.flight.leftPointerEntity;
            var eRightPointer = _contexts.flight.rightPointerEntity;
            var eGuideOffset = _contexts.flight.guideOffsetEntity;

            var result = !eLeftPointer.isEnabled && !eRightPointer.isEnabled;

            if (result && result != eGuideOffset.isEnabled)
            {
                eGuideOffset.offset.Value = 0;
                if (eLeftPointer.offset.Value > eRightPointer.offset.Value) eGuideOffset.direction.Value.x = 1;
                else eGuideOffset.direction.Value.x = -1;
            }

            eGuideOffset.isEnabled = result;
        }
    }
}