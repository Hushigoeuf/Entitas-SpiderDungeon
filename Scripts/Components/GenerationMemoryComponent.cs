using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace GameEngine
{
    [Environment]
    [Unique]
    public class GenerationMemoryComponent : IComponent
    {
        public List<GameObject> PrefabList;
        public bool DecreaseDifficulty;
        public bool[] DiamondFreeSpaces;
    }
}