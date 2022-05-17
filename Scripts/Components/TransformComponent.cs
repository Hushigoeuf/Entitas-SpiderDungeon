using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class TransformComponent : IComponent
    {
        public Transform Value;
    }
}