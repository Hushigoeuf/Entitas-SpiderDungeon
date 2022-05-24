using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки префаба крови или остантков.
    /// </summary>
    [Serializable]
    public sealed class BloodPrefab
    {
        /// Тип препятствия, после столкновения с которым, этот префаб будет создан
        public int ObstacleTypeIndex;

        /// Целевой префаб
        public GameObject Prefab;

        /// Не будет менять позицию объекта в соответствии с персонажем
        public bool IgnorePosition;

        /// Не будет менять угол объекта в соответствии с персонажем
        public bool IgnoreRotation;

        /// Дополнительный сдвиг для угла наклона создаваемого объекта
        public float OffsetRotation;
    }

#if UNITY_EDITOR
    /// Настройки префаба крови специально для инспектора.
    /// Позволяет выбрать несколько типов к которым принадлежит префаб (сделано только для удобства).
    [Serializable]
    public sealed class EditorBloodPrefab
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

    /// <summary>
    /// Настройки крови и остантков, которые остаются после смерти персонажей.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(BloodSettings))]
    public sealed class BloodSettings : ScriptableObject
    {
        /// Итоговый список префабов крови и остантков
        [ListDrawerSettings(Expanded = false, ShowPaging = false)] [ReadOnly]
        public BloodPrefab[] Prefabs = new BloodPrefab[0];

        private List<BloodPrefab> _result;

        /// <summary>
        /// Возвращает список префабов по заданному типу препятствия.
        /// </summary>
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

        /// <summary>
        /// Обновляет итоговый список префабов из инспектора.
        /// </summary>
        [Button]
        private void EditorUpdateFinalList()
        {
            var result = new Dictionary<int, List<BloodPrefab>>();
            var count = 0;
            foreach (var t in _editorPrefabs)
            foreach (var oi in t.ObstacleTypes)
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
            var i = -1;
            foreach (var oi in result.Keys)
            foreach (var p in result[oi])
            {
                i++;
                Prefabs[i] = p;
            }
        }
#endif
    }
}