using UnityEngine;

namespace GameEngine
{
    public interface IPoolService
    {
        bool Contains(string poolID);
        bool Contains(string poolID, Transform item);

        void Create(string poolID);

        Transform Spawn(string poolID, GameObject prefab);

        void Despawn(string poolID, Transform instance);
    }
}