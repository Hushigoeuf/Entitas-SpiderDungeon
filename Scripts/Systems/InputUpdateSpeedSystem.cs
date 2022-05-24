using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Обновляет скорость движения отряда в зависимости от активности.
    /// </summary>
    public sealed class InputUpdateSpeedSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightSettings _settings;

        public InputUpdateSpeedSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _settings = settings.FlightSettings;
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
                e.ReplaceOffset(e.offset.Value + e.speed.Value * Time.deltaTime);
                if (e.offset.Value > e.limit.Value) e.ReplaceOffset(e.limit.Value);
            }
            else
            {
                e.ReplaceOffset(_settings.InputStartOffset);
            }
        }
    }
}