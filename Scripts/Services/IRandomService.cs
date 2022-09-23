using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public interface IRandomService
    {
        float Range(float min, float max);
        int Range(int min, int max, bool inclusive);
        float Range(Vector2 range, bool inclusive);
        int Range(Vector2Int range, bool inclusive);

        int RandomIndex(int count);

        bool IsChance(float criteria);

        void Shuffle<T>(ref T[] values);
        void Shuffle<T>(ref List<T> values);

        T Choose<T>(params T[] values);
        T[] Choose<T>(T[] values, float[] chances, int minCount = 0);
    }
}