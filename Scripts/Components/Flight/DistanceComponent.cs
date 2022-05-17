using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    public sealed class DistanceComponent : IComponent
    {
        public Vector3 Value;
    }
}