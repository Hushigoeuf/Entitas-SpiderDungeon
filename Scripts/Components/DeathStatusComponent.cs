using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Flight]
    [Cleanup(CleanupMode.DestroyEntity)]
    public class DeathStatusComponent : IComponent
    {
    }
}