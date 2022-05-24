using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    [Config]
    public class DeathCountComponent : IComponent
    {
        public int NoCategoryCount;
        public Dictionary<int, int> CategoryCounts;
    }
}