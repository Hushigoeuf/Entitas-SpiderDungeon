using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Двигает отряд в заданную сторону в ручном режиме.
    /// </summary>
    public sealed class InputMovementSystem : IExecuteSystem
    {
        private Contexts _contexts;

        public InputMovementSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void Execute()
        {
            var eLeftPointer = _contexts.flight.leftPointerEntity;
            var eRightPointer = _contexts.flight.rightPointerEntity;
            var eGuideOffset = _contexts.flight.guideOffsetEntity;

            if (!eLeftPointer.isEnabled && !eRightPointer.isEnabled) return;

            var position = eGuideOffset.transform.Value.position;
            if (eLeftPointer.isEnabled)
                position.x -= eLeftPointer.offset.Value * Time.deltaTime;
            else if (eRightPointer.isEnabled) position.x += eRightPointer.offset.Value * Time.deltaTime;
            eGuideOffset.transform.Value.position = position;
        }
    }
}