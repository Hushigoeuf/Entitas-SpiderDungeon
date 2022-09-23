using Entitas;

namespace GameEngine
{
    public class InputInitSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightSettings _flightSettings;

        public InputInitSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _flightSettings = settings.FlightSettings;
        }

        public void Initialize()
        {
            var e = CreateInputEntity();
            {
                e.isLeftPointer = true;
                e.isControlled = true;
            }

            e = CreateInputEntity();
            {
                e.isRightPointer = true;
                e.isControlled = true;
            }
        }

        private FlightEntity CreateInputEntity()
        {
            var e = _contexts.flight.CreateEntity();
            {
                e.AddRate(0);
                e.AddSpeed(_flightSettings.InputSpeed);
                e.AddOffset(_flightSettings.InputStartOffset);
                e.AddLimit(_flightSettings.InputLimit);
            }
            return e;
        }
    }
}