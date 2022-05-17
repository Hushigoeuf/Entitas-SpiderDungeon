using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Config]
    public sealed class CleanupObjectComponent : IComponent
    {
        public GameObject Value;
    }
}