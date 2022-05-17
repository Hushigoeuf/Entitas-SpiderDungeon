using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Config]
    [Cleanup(CleanupMode.DestroyEntity)]
    public class CoconLifeRequestComponent : IComponent
    {
    }
}