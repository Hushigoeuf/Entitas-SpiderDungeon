using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public class DirectionComponent : IComponent
    {
        public Vector3 Value;
    }
}