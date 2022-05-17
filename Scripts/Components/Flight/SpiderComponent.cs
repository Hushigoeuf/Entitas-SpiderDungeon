using Entitas;

namespace GameEngine
{
    [Flight]
    public sealed class SpiderComponent : IComponent
    {
        public FlightEntity FollowEntity;
        public FlightEntity RotationEntity;
    }
}