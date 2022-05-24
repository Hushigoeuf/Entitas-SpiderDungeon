using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Config]
    [Environment]
    [Flight]
    [Cleanup(CleanupMode.DestroyEntity)]
    public class CleanupComponent : IComponent
    {
    }
}