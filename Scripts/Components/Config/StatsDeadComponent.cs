using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class StatsDeadComponent : IComponent
    {
        public int NoCategoryCount;
        public Dictionary<int, int> CategoryCountGroupList;
    }
}