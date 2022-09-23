using System;
using Sirenix.OdinInspector;
using UnityEngine;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;

#endif

namespace GameEngine
{
    [Serializable]
    public class TrapPrefab
    {
#if UNITY_EDITOR
        [ShowInInspector]
        [HideLabel]
        [ReadOnly]
        private string EditorPrefabName => Prefab?.name;
#endif

        [PropertyOrder(100)] [PreviewField(256, ObjectFieldAlignment.Center)] [Required] [HideLabel]
        public GameObject Prefab;

        [PropertyOrder(101)] public bool UseOnlyGlobalChecker = true;

        [PropertyOrder(102)] [FoldoutGroup(nameof(Checker), false)] [DisableIf(nameof(UseOnlyGlobalChecker))]
        public TrapChecker Checker = new TrapChecker();

        [HideInInspector] public int[] Position = new int[4];

        [HideInInspector] public bool[] DiamondFreeSpaces = new bool[4];

#if UNITY_EDITOR
        [Flags]
        private enum EditorTrapPositionEnum
        {
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            ALL = A | B | C | D
        }

        [PropertyOrder(200)]
        [EnumToggleButtons]
        [ShowInInspector]
        [HideLabel]
        private EditorTrapPositionEnum EditorPosition
        {
            get
            {
                var result = EditorTrapPositionEnum.ALL;
                if (Position[0] == 0) result ^= EditorTrapPositionEnum.A;
                if (Position[1] == 0) result ^= EditorTrapPositionEnum.B;
                if (Position[2] == 0) result ^= EditorTrapPositionEnum.C;
                if (Position[3] == 0) result ^= EditorTrapPositionEnum.D;
                return result;
            }
            set
            {
                Position[0] = value.HasFlag(EditorTrapPositionEnum.A) ? 1 : 0;
                Position[1] = value.HasFlag(EditorTrapPositionEnum.B) ? 1 : 0;
                Position[2] = value.HasFlag(EditorTrapPositionEnum.C) ? 1 : 0;
                Position[3] = value.HasFlag(EditorTrapPositionEnum.D) ? 1 : 0;
            }
        }

        [PropertyOrder(201)]
        [EnumToggleButtons]
        [ShowInInspector]
        [HideLabel]
        private EditorTrapPositionEnum EditorFreeSpaces
        {
            get
            {
                var result = EditorTrapPositionEnum.ALL;
                if (Position[0] <= 1) result ^= EditorTrapPositionEnum.A;
                if (Position[1] <= 1) result ^= EditorTrapPositionEnum.B;
                if (Position[2] <= 1) result ^= EditorTrapPositionEnum.C;
                if (Position[3] <= 1) result ^= EditorTrapPositionEnum.D;
                return result;
            }
            set
            {
                Position[0] = value.HasFlag(EditorTrapPositionEnum.A) ? 2 : Position[0] != 0 ? 1 : 0;
                Position[1] = value.HasFlag(EditorTrapPositionEnum.B) ? 2 : Position[1] != 0 ? 1 : 0;
                Position[2] = value.HasFlag(EditorTrapPositionEnum.C) ? 2 : Position[2] != 0 ? 1 : 0;
                Position[3] = value.HasFlag(EditorTrapPositionEnum.D) ? 2 : Position[3] != 0 ? 1 : 0;
            }
        }

        [PropertyOrder(202)]
        [TitleGroup(nameof(DiamondFreeSpaces))]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_0
        {
            get => DiamondFreeSpaces[0];
            set => DiamondFreeSpaces[0] = value;
        }

        [PropertyOrder(203)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_1
        {
            get => DiamondFreeSpaces[1];
            set => DiamondFreeSpaces[1] = value;
        }

        [PropertyOrder(204)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_2
        {
            get => DiamondFreeSpaces[2];
            set => DiamondFreeSpaces[2] = value;
        }

        [PropertyOrder(205)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_3
        {
            get => DiamondFreeSpaces[3];
            set => DiamondFreeSpaces[3] = value;
        }

        [PropertyOrder(200 - 10)]
        [OnInspectorGUI]
        private void EditorBeginPosition()
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginIndentedHorizontal();
            GUILayout.Label(nameof(Position));
            SirenixEditorGUI.BeginIndentedVertical();
        }

        [PropertyOrder(200 + 10)]
        [OnInspectorGUI]
        private void EditorEndPosition()
        {
            SirenixEditorGUI.EndIndentedVertical();
            SirenixEditorGUI.EndIndentedHorizontal();
            SirenixEditorGUI.EndBox();
        }
#endif
    }

    [Serializable]
    [HideLabel]
    public class TrapChecker
    {
        [Range(0, 1)] public float Chance = 1;
        [MinValue(0)] public int StartAtIndex;
        [MinValue(0)] public int StartAtScore;
        [Range(0, 5)] public int MinRegionIndex;
    }

    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(TrapTypeSettings))]
    public class TrapTypeSettings : ScriptableObject
    {
        public TrapChecker Checker = new TrapChecker();

        public CountParameter DeathCounter;

        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = true, ShowIndexLabels = false,
            NumberOfItemsPerPage = 1)]
        [Space(12)]
        public TrapPrefab[] Prefabs = new TrapPrefab[0];
    }
}