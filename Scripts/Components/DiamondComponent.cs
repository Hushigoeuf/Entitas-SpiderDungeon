using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Environment]
    [Cleanup(CleanupMode.DestroyEntity)]
    public class DiamondComponent : IComponent
    {
    }
}