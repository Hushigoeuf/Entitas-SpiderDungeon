using Entitas;

namespace GameEngine
{
    [Environment]
    public sealed class EntityEnvironmentComponent : IComponent
    {
        public EnvironmentEntity Value;
    }
}