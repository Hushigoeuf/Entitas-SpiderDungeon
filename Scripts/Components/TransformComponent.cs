using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public class TransformComponent : IComponent
    {
        public Transform Value;
    }
}