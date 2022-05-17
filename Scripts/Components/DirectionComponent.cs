using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class DirectionComponent : IComponent
    {
        public Vector3 Value;
    }
}