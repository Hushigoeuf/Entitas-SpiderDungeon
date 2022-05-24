using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Environment]
    [Config]
    public class PositionComponent : IComponent
    {
        public Vector3 Value;
    }
}