using PathologicalGames;
using UnityEngine;

namespace GameEngine
{
    public sealed class PathologicalPoolService : IPoolService
    {
        public bool Contains(string poolId)
        {
            return PoolManager.Pools.ContainsKey(poolId);
        }

        public bool Contains(string poolId, Transform item)
        {
            return PoolManager.Pools[poolId].IsSpawned(item);
        }

        public void Create(string poolId)
        {
            PoolManager.Pools.Create(poolId);
        }

        public Transform Spawn(string poolId, GameObject prefab)
        {
            return PoolManager.Pools[poolId].Spawn(prefab);
        }

        public void Despawn(string poolId, Transform instance)
        {
            PoolManager.Pools[poolId].Despawn(instance);
        }
    }
}