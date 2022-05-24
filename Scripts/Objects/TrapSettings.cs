using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки препятствий.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(TrapSettings))]
    public sealed class TrapSettings : ScriptableObject
    {
        /// Дистанция между ловушками в процессе генерации
        [MinValue(0)] public float GenerationStepSize;

        /// Сколько надо набрать очков чтобы перейти на следующий этап
        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false)]
        public int[] ScoreRegions = new int[0];

        /// Список всех типов препятствий
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = false, ShowIndexLabels = false)]
        public TrapTypeSettings[] Types = new TrapTypeSettings[0];

#if UNITY_EDITOR
        [Button]
        private void EditorLoadAllTypes()
        {
            Types = Resources.LoadAll<TrapTypeSettings>("");
        }
#endif
    }
}