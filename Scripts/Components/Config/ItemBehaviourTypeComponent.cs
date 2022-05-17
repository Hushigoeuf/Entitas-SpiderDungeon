using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class ItemBehaviourTypeComponent : IComponent
    {
        public ItemBehaviourTypes Value;
    }
}