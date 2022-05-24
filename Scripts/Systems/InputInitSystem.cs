using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Иниализирует сущности для управления отрядом.
    /// </summary>
    public sealed class InputInitSystem : IInitializeSystem
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
            // Создает сущность, чтобы двигать персонажей в левую сторону
            var e = _contexts.flight.CreateEntity();
            {
                e.isLeftPointer = true;
                e.isControlled = true;
                e.AddRate(0);
                e.AddSpeed(_flightSettings.InputSpeed);
                e.AddOffset(_flightSettings.InputStartOffset);
                e.AddLimit(_flightSettings.InputLimit);
            }

            // Создает сущность, чтобы двигать персонажей в правую сторону
            e = _contexts.flight.CreateEntity();
            {
                e.isRightPointer = true;
                e.isControlled = true;
                e.AddRate(0);
                e.AddSpeed(_flightSettings.InputSpeed);
                e.AddOffset(_flightSettings.InputStartOffset);
                e.AddLimit(_flightSettings.InputLimit);
            }
        }
    }
}