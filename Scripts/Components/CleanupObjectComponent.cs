using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Config]
    public class CleanupObjectComponent : IComponent
    {
        public GameObject Value;
    }
}