using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class GuideMovementSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;

        public GuideMovementSystem(Contexts contexts)
        {
            _contexts = contexts;
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
                e.offset.Value -= e.speed.Value * Time.deltaTime;
                if (e.offset.Value <= -e.limit.Value)
                {
                    e.offset.Value = -e.limit.Value;
                    e.direction.Value.x = 1;
                }
            }
            else if (e.direction.Value.x == 1)
            {
                e.offset.Value += e.speed.Value * Time.deltaTime;
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
            position.x += e.offset.Value * Time.deltaTime;
            e.transform.Value.position = position;
        }
    }
}