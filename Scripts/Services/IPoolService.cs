using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Паттерн сервиса для работы с пулом объектов.
    /// </summary>
    public interface IPoolService
    {
        /// Существует ли пул объектов
        bool Contains(string poolID);

        /// Существует ли объект в заданном пуле
        bool Contains(string poolID, Transform item);

        /// Создает новый пул объектов по ID
        void Create(string poolID);

        /// Создает новый объект из префаба в заданном пуле
        Transform Spawn(string poolID, GameObject prefab);

        /// Уничтожает объект из существующего пула объектов
        void Despawn(string poolID, Transform instance);
    }
}