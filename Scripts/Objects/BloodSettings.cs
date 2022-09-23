using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [Serializable]
    public class BloodPrefab
    {
        public int ObstacleTypeIndex;
        public GameObject Prefab;
        public bool IgnorePosition;
        public bool IgnoreRotation;
        public float OffsetRotation;
    }

#if UNITY_EDITOR
    [Serializable]
    public class EditorBloodPrefab
    {
        public static ValueDropdownList<int> EditorObstacleTypes = new ValueDropdownList<int>()
        {
            {"Default", 0},
            {"SideLeft", 1},
            {"SideRight", 2},
            {"Fire", 3},
            {"Lightning", 4},
        };

        [Required] public GameObject Prefab;

        public bool IgnorePosition;

        public bool IgnoreRotation;

        [DisableIf(nameof(IgnoreRotation))] public float OffsetRotation;

        [ListDrawerSettings(Expanded = true, ShowPaging = false)]
        [ValueDropdown(nameof(EditorObstacleTypes))]
        [SerializeField]
        public int[] ObstacleTypes = new int[0];
    }
#endif

    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(BloodSettings))]
    public class BloodSettings : ScriptableObject
    {
        [ListDrawerSettings(Expanded = false, ShowPaging = false)] [ReadOnly]
        public BloodPrefab[] Prefabs = new BloodPrefab[0];

        private List<BloodPrefab> _result;

        public List<BloodPrefab> GetPrefabs(int obstacleTypeIndex)
        {
            if (_result == null)
                _result = new List<BloodPrefab>();
            else _result.Clear();
            for (var i = 0; i < Prefabs.Length; i++)
                if (Prefabs[i].ObstacleTypeIndex == obstacleTypeIndex)
                    _result.Add(Prefabs[i]);
            return _result;
        }

#if UNITY_EDITOR
        [ListDrawerSettings(Expanded = true, ShowPaging = false)] [SerializeField]
        private EditorBloodPrefab[] _editorPrefabs = new EditorBloodPrefab[0];

        [Button]
        private void EditorUpdateFinalList()
        {
            var result = new Dictionary<int, List<BloodPrefab>>();
            var count = 0;
            foreach (var t in _editorPrefabs)
            foreach (int oi in t.ObstacleTypes)
            {
                if (!result.ContainsKey(oi)) result.Add(oi, new List<BloodPrefab>());
                count++;
                result[oi].Add(new BloodPrefab()
                {
                    ObstacleTypeIndex = oi,
                    Prefab = t.Prefab,
                    IgnorePosition = t.IgnorePosition,
                    IgnoreRotation = t.IgnoreRotation,
                    OffsetRotation = t.OffsetRotation,
                });
            }

            Prefabs = new BloodPrefab[count];
            int i = -1;
            foreach (int oi in result.Keys)
            foreach (var p in result[oi])
            {
                i++;
                Prefabs[i] = p;
            }
        }
#endif
    }
}