using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class TargetComponent : IComponent
    {
        public Transform Value;
    }
}