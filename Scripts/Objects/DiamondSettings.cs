using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки с префабом алмаза.
    /// </summary>
    [Serializable]
    public struct DiamondPrefab
    {
        /// Целевой префаб алмаза
        [AssetsOnly] [Required] public GameObject Prefab;

        /// Шанс выпадения этого префаба 
        [Range(0, 1)] public float Chance;

        /// Сколько фиатных алмазов дается игроку за сбор этого алмаза
        [MinValue(1)] public int CostSize;
    }

    /// <summary>
    /// Настройки алмазов.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(DiamondSettings))]
    public sealed class DiamondSettings : ScriptableObject
    {
        private const string EDITOR_GROUP_GENERAL = "General";
        private const string EDITOR_GROUP_DROP = "Drop";

        /// Дистанция между алмазами
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float DistancePerDiamond;

        /// Префаб стандартного алмаза
        [BoxGroup(EDITOR_GROUP_GENERAL)] [AssetsOnly] [Required]
        public GameObject DefaultPrefab;

        /// Сколько фиатных алмазов дается за стандартный алмаз
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(1)]
        public int DefaultCostSize;

        /// Префаб с VFX, который будет создан после сбора алмаза
        [BoxGroup(EDITOR_GROUP_GENERAL)] [AssetsOnly] [Required]
        public GameObject ExplosionPrefab;

        /// Дополнительные префабы алмазов
        [BoxGroup(EDITOR_GROUP_GENERAL)] [ListDrawerSettings(Expanded = true, ShowPaging = false)]
        public DiamondPrefab[] OtherPrefabList = new DiamondPrefab[0];

        /// Минимальное кол-во алмазов, которое будет создана генератором во время своей итерации
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountPerOnce))]
        public int MinDropCountPerOnce;

        /// Максимальное кол-во алмазов (см. выше)
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountPerOnce))]
        public int MaxDropCountPerOnce;

        /// Шанс того, что генератор разделит спавн алмазов на несколько проходов
        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float SplitChance;

        /// Минимальное кол-во алмазов, которые будут созданы в одном из проходов в случае разделения
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountPerSplit))]
        public int MinDropCountPerSplit;

        /// Максимальное кол-во алмазов (см. выше)
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountPerSplit))]
        public int MaxDropCountPerSplit;

        /// Шанс того, что генератор создаст только алмазы в своей итерации и поместит их горизонтально
        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float HorizontalDropChance;

        /// Минимальное кол-во алмазов для горизонтального спавна
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountForHorizontal))]
        public int MinDropCountForHorizontal;

        /// Максимальное кол-во алмазов (см. выше)
        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountForHorizontal))]
        public int MaxDropCountForHorizontal;

        /// Шанс разделить горизонтальный спавн на несколько "этажей"
        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float HorizontalSplitChance;
    }
}