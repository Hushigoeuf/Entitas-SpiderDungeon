using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(TrapSettings))]
    public class TrapSettings : ScriptableObject
    {
        [MinValue(0)] public float GenerationStepSize;

        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false)]
        public int[] ScoreRegions = new int[0];

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