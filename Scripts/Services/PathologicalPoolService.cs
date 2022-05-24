using PathologicalGames;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Сервис для работы с пулом объектов на основе PoolManager.
    /// Источник: http://poolmanager.path-o-logical.com/
    /// </summary>
    public sealed class PathologicalPoolService : IPoolService
    {
        public bool Contains(string poolID) => PoolManager.Pools.ContainsKey(poolID);

        public bool Contains(string poolID, Transform item) => PoolManager.Pools[poolID].IsSpawned(item);

        public void Create(string poolID)
        {
            PoolManager.Pools.Create(poolID);
        }

        public Transform Spawn(string poolID, GameObject prefab) => PoolManager.Pools[poolID].Spawn(prefab);

        public void Despawn(string poolID, Transform instance)
        {
            PoolManager.Pools[poolID].Despawn(instance);
        }
    }
}