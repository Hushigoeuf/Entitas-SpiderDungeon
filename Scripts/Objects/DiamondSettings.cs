using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [Serializable]
    public struct DiamondPrefab
    {
        [AssetsOnly] [Required] public GameObject Prefab;
        [Range(0, 1)] public float Chance;
        [MinValue(1)] public int CostSize;
    }

    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(DiamondSettings))]
    public class DiamondSettings : ScriptableObject
    {
        private const string EDITOR_GROUP_GENERAL = "General";
        private const string EDITOR_GROUP_DROP = "Drop";

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float DistancePerDiamond;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [AssetsOnly] [Required]
        public GameObject DefaultPrefab;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(1)]
        public int DefaultCostSize;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [AssetsOnly] [Required]
        public GameObject ExplosionPrefab;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [ListDrawerSettings(Expanded = true, ShowPaging = false)]
        public DiamondPrefab[] OtherPrefabList = new DiamondPrefab[0];

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountPerOnce))]
        public int MinDropCountPerOnce;

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountPerOnce))]
        public int MaxDropCountPerOnce;

        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float SplitChance;

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountPerSplit))]
        public int MinDropCountPerSplit;

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountPerSplit))]
        public int MaxDropCountPerSplit;

        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float HorizontalDropChance;

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(0)] [MaxValue(nameof(MaxDropCountForHorizontal))]
        public int MinDropCountForHorizontal;

        [BoxGroup(EDITOR_GROUP_DROP)] [MinValue(nameof(MinDropCountForHorizontal))]
        public int MaxDropCountForHorizontal;

        [BoxGroup(EDITOR_GROUP_DROP)] [Range(0, 1)]
        public float HorizontalSplitChance;
    }
}