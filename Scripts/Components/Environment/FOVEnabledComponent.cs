using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Environment]
    [Event(EventTarget.Self, EventType.Added)]
    public sealed class FOVEnabledComponent : IComponent
    {
    }
}