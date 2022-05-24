using Entitas;

namespace GameEngine
{
    [Config]
    public class InventoryItemTypeComponent : IComponent
    {
        public InventoryItemTypes Value;
    }
}