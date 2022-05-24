using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Паттерн сервиса для рандомизирования данных.
    /// </summary>
    public interface IRandomService
    {
        /// Возвращает случайное значение между минимальным и максимальным
        float Range(float min, float max);

        int Range(int min, int max, bool inclusive);
        float Range(Vector2 range, bool inclusive);
        int Range(Vector2Int range, bool inclusive);

        /// Возвращает случайный индекс на основе кол-ва
        int RandomIndex(int count);

        /// Возвращает статус выпадение по заданному шансу (0-1)
        bool IsChance(float criteria);

        /// Перемешивает массив объктов случайным образом
        void Shuffle<T>(ref T[] values);

        void Shuffle<T>(ref List<T> values);

        /// Возвращает случайный элемент массива
        T Choose<T>(params T[] values);

        /// Возвращает рандомизированный массив из другого с учатом вероятностей
        T[] Choose<T>(T[] values, float[] chances, int minCount = 0);
    }
}