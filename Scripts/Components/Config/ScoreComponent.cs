using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace GameEngine
{
    [Config]
    public sealed class ScoreComponent : IComponent
    {
        public int Value;
    }
}