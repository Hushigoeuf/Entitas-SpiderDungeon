using UnityEngine;

namespace GameEngine
{
    public interface IPoolService
    {
        bool Contains(string poolId);
        bool Contains(string poolId, Transform item);
        void Create(string poolId);
        Transform Spawn(string poolId, GameObject prefab);
        void Despawn(string poolId, Transform instance);
    }
}