using Entitas;

namespace GameEngine
{
    public sealed class InputRateSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;

        public InputRateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void Execute()
        {
            ExecuteEntity(_contexts.flight.leftPointerEntity);
            ExecuteEntity(_contexts.flight.rightPointerEntity);
        }

        private void ExecuteEntity(FlightEntity entity)
        {
            if (!entity.isEnabled)
            {
                if (entity.hasRate && entity.rate.Value != 0)
                    entity.ReplaceRate(0);
                return;
            }

            if (!entity.hasRate) return;

            if (entity.rate.Value != 1) entity.ReplaceRate(1);
        }
    }
}