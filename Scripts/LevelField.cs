using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [Serializable]
    public struct CountLevelField
    {
        [MinValue(0)] public int StartValue;
        [MinValue(1)] public int LevelSize;
        [MinValue(0)] public int ChangeValuePerLevel;

        public int GetCount(int level, int min = 0, int max = int.MaxValue)
        {
            var result = 0;
            if (level > 0) result = StartValue - Mathf.FloorToInt((float) level / LevelSize) * ChangeValuePerLevel;
            return Mathf.Clamp(result, min, max);
        }
    }

    [Serializable]
    public struct ChanceLevelField
    {
        [Range(0, 1)] public float StartValue;
        [MinValue(1)] public int LevelSize;
        [Range(0, 1)] public float ChangeValuePerLevel;

        public float GetChance(int level)
        {
            var result = 0f;
            if (level > 0) result = StartValue - Mathf.FloorToInt((float) level / LevelSize) * ChangeValuePerLevel;
            return Mathf.Clamp(result, 0, 1);
        }
    }

    [Serializable]
    public struct TimeLevelField
    {
        [MinValue(0)] public float StartValue;
        [MinValue(1)] public int LevelSize;
        [MinValue(0)] public float ChangeValuePerLevel;

        public float GetTime(int level)
        {
            var result = 0f;
            if (level > 0) result = StartValue - Mathf.FloorToInt((float) level / LevelSize) * ChangeValuePerLevel;
            return Mathf.Clamp(result, 0, float.MaxValue);
        }
    }
}