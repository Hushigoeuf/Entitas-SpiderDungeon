using Entitas;

namespace GameEngine
{
    [Flight]
    public class CharacterComponent : IComponent
    {
        public FlightEntity FollowEntity;
        public FlightEntity RotationEntity;
    }
}