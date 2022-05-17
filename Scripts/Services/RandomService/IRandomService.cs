using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public interface IRandomService
    {
        // Выдает значение между заданным диапозоном, где inclusive - является ли max включительным.
        // Для Vector2/Vector2Int x и y соответствуют min и max.
        float Range(float min, float max, bool inclusive);
        int Range(int min, int max, bool inclusive);
        float Range(Vector2 range, bool inclusive);
        int Range(Vector2Int range, bool inclusive);

        // Выдает да/нет на основе заданного шанса (от 0.0 до 1.0).
        bool IsChance(float criteria);
        float Point();
        
        // Выдает случайный индекс на основе заданного кол-ва элементов массива или листа.
        int Index(int count);

        // Перемешивает массив случайным образом.
        void Shuffle<T>(ref T[] values);
        void Shuffle<T>(ref List<T> values);

        // Выдает случайный элемент из совокупности элементов.
        T Choose<T>(params T[] values);

        // Выдает массив случайных значений с учетом вероятностей.
        T[] Choose<T>(T[] values, float[] chances, int minCount = 0);
    }
}