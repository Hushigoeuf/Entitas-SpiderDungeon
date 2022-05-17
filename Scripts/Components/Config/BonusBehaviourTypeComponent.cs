using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class BonusBehaviourTypeComponent : IComponent
    {
        public BonusBehaviourTypes Value;
    }
}