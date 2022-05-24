using System;
using Sirenix.OdinInspector;
using UnityEngine;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;

#endif

namespace GameEngine
{
    /// <summary>
    /// Настройки префаба и условий для его создания в процессе генерации.
    /// </summary>
    [Serializable]
    public sealed class TrapPrefab
    {
        /// Отображает имя префаба если он задан
#if UNITY_EDITOR
        [ShowInInspector]
        [HideLabel]
        [ReadOnly]
        private string EditorPrefabName => Prefab?.name;
#endif

        /// Целевой префаб препятствия
        [PropertyOrder(100)] [PreviewField(256, ObjectFieldAlignment.Center)] [Required] [HideLabel]
        public GameObject Prefab;

        /// Использовать только глобальный чекер
        [PropertyOrder(101)] public bool UseOnlyGlobalChecker = true;

        /// Локальный чекер в рамках одного префаба
        [PropertyOrder(102)] [FoldoutGroup(nameof(Checker), false)] [DisableIf(nameof(UseOnlyGlobalChecker))]
        public TrapChecker Checker = new TrapChecker();

        /// Информация о позиции препятствия в пространстве
        [HideInInspector] public int[] Position = new int[4];

        /// Информация о возможном положении алмазов в зоне препятствия
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

        /// Отображает позицию препятствия в инспекторе в виде EnumToggleButtons.
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

        /// Отображает проходы в зоне препятствия в виде EnumToggleButtons.
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

        /// Возможно ли сгенерировать алмазы в 1 проходе (уровень по горизонтали делится на 4 прохода)
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

        /// Возможно ли сгенерировать алмазы во 2 проходе
        [PropertyOrder(203)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_1
        {
            get => DiamondFreeSpaces[1];
            set => DiamondFreeSpaces[1] = value;
        }

        /// Возможно ли сгенерировать алмазы в 3 проходе
        [PropertyOrder(204)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_2
        {
            get => DiamondFreeSpaces[2];
            set => DiamondFreeSpaces[2] = value;
        }

        /// Возможно ли сгенерировать алмазы в 4 проходе
        [PropertyOrder(205)]
        [HorizontalGroup(nameof(DiamondFreeSpaces) + "/" + nameof(EditorDiamondFreeSpaces_0))]
        [HideLabel]
        [ShowInInspector]
        private bool EditorDiamondFreeSpaces_3
        {
            get => DiamondFreeSpaces[3];
            set => DiamondFreeSpaces[3] = value;
        }

        /// <summary>
        /// Оформляет EnumToggleButtons для позиции.
        /// </summary>
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

    /// <summary>
    /// Класс-чекер с условиями для отбора.
    /// </summary>
    [Serializable]
    [HideLabel]
    public sealed class TrapChecker
    {
        /// Шанс выпадения от 0 до 1
        [Range(0, 1)] public float Chance = 1;

        /// С какой итерации генератора можно внести этот тип в отбор
        [MinValue(0)] public int StartAtIndex;

        /// С какого игрового счета можно внести этот тип в отбор
        [MinValue(0)] public int StartAtScore;

        /// С какого этапа можно внести этот тип в отбор
        [Range(0, 5)] public int MinRegionIndex;
    }

    /// <summary>
    /// Объект типа для препятствий, содержит настройки типа.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(TrapTypeSettings))]
    public sealed class TrapTypeSettings : ScriptableObject
    {
        /// Основной чекер для прохождения отбора
        public TrapChecker Checker = new TrapChecker();

        /// Счетчик смертей от этого типа
        public CountParameter DeathCounter;

        /// Список всех префабов, которые относятся к этому типу
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = true, ShowIndexLabels = false,
            NumberOfItemsPerPage = 1)]
        [Space(12)]
        public TrapPrefab[] Prefabs = new TrapPrefab[0];
    }
}