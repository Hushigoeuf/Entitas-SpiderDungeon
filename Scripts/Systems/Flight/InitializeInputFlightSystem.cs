using System;
using Entitas;

namespace GameEngine
{
    public sealed class InitializeInputFlightSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightData _flightData;
        
        public InitializeInputFlightSystem(Contexts contexts, Data data)
        {
            _contexts = contexts;
            _flightData = data.FlightData;
        }

        public void Initialize()
        {
            var e = _contexts.flight.CreateEntity();
            {
                e.isLeftPointer = true;
                e.isControlled = true;
                e.AddRate(0);
                e.AddSpeed(_flightData.InputSpeed);
                e.AddOffset(_flightData.InputStartOffset);
                e.AddLimit(_flightData.InputLimit);
            }

            e = _contexts.flight.CreateEntity();
            {
                e.isRightPointer = true;
                e.isControlled = true;
                e.AddRate(0);
                e.AddSpeed(_flightData.InputSpeed);
                e.AddOffset(_flightData.InputStartOffset);
                e.AddLimit(_flightData.InputLimit);
            }
        }
    }
}