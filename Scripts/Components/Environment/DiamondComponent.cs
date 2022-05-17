using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Environment]
    [Cleanup(CleanupMode.DestroyEntity)]
    public sealed class DiamondComponent : IComponent
    {
    }
}