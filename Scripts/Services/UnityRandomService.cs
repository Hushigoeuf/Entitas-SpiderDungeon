using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class UnityRandomService : IRandomService
    {
        public float Range(float min, float max)
        {
            min = Mathf.Clamp(min, int.MinValue, max);
            max = Mathf.Clamp(max, min, int.MaxValue);
            return Random.Range(min, max);
        }

        public int Range(int min, int max, bool inclusive)
        {
            min = Mathf.Clamp(min, int.MinValue, max);
            max = Mathf.Clamp(max, min, int.MaxValue);
            return min != max ? Random.Range(min, max + (inclusive ? 1 : 0)) : min;
        }

        public float Range(Vector2 range, bool inclusive) => Range(range.x, range.y);

        public int Range(Vector2Int range, bool inclusive) => Range(range.x, range.y, inclusive);

        public int RandomIndex(int count) => Range(0, Mathf.Clamp(count, 0, int.MaxValue), false);

        public bool IsChance(float criteria) => Range(0f, 1f) < Mathf.Clamp01(criteria);

        public void Shuffle<T>(ref T[] values)
        {
            if (values == null || values.Length == 0) return;
            var random = new System.Random();
            for (int i = values.Length - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                (values[j], values[i]) = (values[i], values[j]);
            }
        }

        public void Shuffle<T>(ref List<T> values)
        {
            if (values == null || values.Count == 0) return;
            var random = new System.Random();
            for (int i = values.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                (values[j], values[i]) = (values[i], values[j]);
            }
        }

        public T Choose<T>(params T[] values) => values[Range(0, values.Length, false)];

        public T[] Choose<T>(T[] values, float[] chances, int minCount = 0)
        {
            var sum = 0f;
            var max = float.MinValue;
            var indexes = new int[values.Length];
            {
                for (var i = 0; i < chances.Length; i++)
                {
                    sum += chances[i];
                    if (chances[i] > max) max = chances[i];
                    indexes[i] = i;
                }

                float criteria = sum / max;
                if (criteria < 1f) criteria = 1f;
                for (var i = 0; i < chances.Length; i++) chances[i] = chances[i] / max / criteria;

                sum = -.000002f;
                max = float.MinValue;
                for (var i = 0; i < chances.Length; i++)
                {
                    sum += chances[i];
                    if (chances[i] > max) max = chances[i];
                }
            }

            var result = new List<T>();
            {
                float point = Random.value * sum;

                Shuffle(ref indexes);

                foreach (int index in indexes)
                    if (point < chances[index])
                        result.Add(values[index]);
                    else point -= chances[index];

                if (result.Count < minCount)
                    while (result.Count < minCount)
                    {
                        var newResult = Choose<T>(values, chances);
                        for (var i = 0; i < newResult.Length; i++) result.Add(newResult[i]);
                    }
            }

            var resultArr = new T[result.Count];
            for (var i = 0; i < result.Count; i++) resultArr[i] = result[i];
            return resultArr;
        }
    }
}