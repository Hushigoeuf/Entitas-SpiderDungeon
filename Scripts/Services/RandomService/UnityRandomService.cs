using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public sealed class UnityRandomService : IRandomService
    {
        public float Range(float min, float max, bool inclusive)
        {
#if !GE_DEBUG_DISABLED
            if (max < min) throw new CustomArgumentException();
#endif
            return min != max ? Random.Range(min, max) : min;
        }

        public int Range(int min, int max, bool inclusive)
        {
#if !GE_DEBUG_DISABLED
            if (max < min) throw new CustomArgumentException();
#endif
            return min != max ? Random.Range(min, max + (inclusive ? 1 : 0)) : min;
        }

        public float Range(Vector2 range, bool inclusive)
        {
#if !GE_DEBUG_DISABLED
            if (range == null) throw new CustomArgumentException();
#endif
            return Range(range.x, range.y, inclusive);
        }

        public int Range(Vector2Int range, bool inclusive)
        {
#if !GE_DEBUG_DISABLED
            if (range == null) throw new CustomArgumentException();
#endif
            return Range(range.x, range.y, inclusive);
        }

        public bool IsChance(float criteria)
        {
#if !GE_DEBUG_DISABLED
            if (criteria < .0f || criteria > 1f) throw new CustomArgumentException();
#endif
            if (criteria > .0f && criteria < 1f) return Range(.0f, 1f, true) < criteria;
            return criteria == 1f;
        }

        public int Index(int count)
        {
#if !GE_DEBUG_DISABLED
            if (count <= 0) throw new CustomArgumentException();
#endif
            return Range(0, count, false);
        }

        public void Shuffle<T>(ref T[] values)
        {
            if (values == null || values.Length == 0) return;
            var random = new System.Random();
            for (var i = values.Length - 1; i >= 1; i--)
            {
                var j = random.Next(i + 1);
                var temp = values[j];
                values[j] = values[i];
                values[i] = temp;
            }
        }

        public void Shuffle<T>(ref List<T> values)
        {
            if (values == null || values.Count == 0) return;
            var random = new System.Random();
            for (var i = values.Count - 1; i >= 1; i--)
            {
                var j = random.Next(i + 1);
                var temp = values[j];
                values[j] = values[i];
                values[i] = temp;
            }
        }

        public T Choose<T>(params T[] values)
        {
#if !GE_DEBUG_DISABLED
            if (values == null || values.Length == 0) throw new CustomArgumentException();
#endif
            return values[Range(0, values.Length, false)];
        }

        public T[] Choose<T>(T[] values, float[] chances, int minCount = 0)
        {
#if !GE_DEBUG_DISABLED
            if (values == null || chances == null || values.Length != chances.Length || values.Length == 0)
                throw new CustomArgumentException();
#endif

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

                var criteria = sum / max;
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
                var point = Random.value * sum;

                Shuffle(ref indexes);

                foreach (var index in indexes)
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

        public float Point()
        {
            return Random.value;
        }
    }
}