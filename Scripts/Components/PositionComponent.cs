using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Environment]
    [Config]
    public sealed class PositionComponent : IComponent
    {
        public Vector3 Value;
    }
}