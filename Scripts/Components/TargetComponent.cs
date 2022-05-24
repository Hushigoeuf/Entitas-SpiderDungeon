using Entitas;
using UnityEngine;

namespace GameEngine
{
    [Flight]
    [Environment]
    public class TargetComponent : IComponent
    {
        public Transform Value;
    }
}