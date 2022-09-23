using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(ItemSettings))]
    public class ItemSettings : ScriptableObject
    {
        #region AdditionalLife

        [FoldoutGroup(nameof(InventoryItemTypes.AdditionalLife))]
        public CountLevelField LifeDropCount;

        #endregion

        #region Luck

        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public ChanceLevelField LuckDropChance;

        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public CountLevelField LuckDropPause;

        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public TimeLevelField LuckDropSpeed;

        #endregion

        #region Cocoon

        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))] [Required]
        public GameObject CocoonPrefab;

        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public ChanceLevelField CocoonDropChance;

        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))] [MinValue(0)]
        public int CocoonPauseAfterStart;

        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public CountLevelField CocoonDropPause;

        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public CountLevelField CocoonDropLife;

        #endregion

        #region Shield

        [FoldoutGroup(nameof(InventoryItemTypes.Shield))]
        public ChanceLevelField ShieldDropChance;

        #endregion

        #region Sonar

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public ChanceLevelField SonarDropChance;

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public CountLevelField SonarDropCount;

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public TimeLevelField SonarDropPause;

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(0)]
        public float SonarOffset;

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(.1f)]
        public float SonarAnimationDuration;

        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(.1f)]
        public float SonarAnimationPause;

        #endregion

        #region TimeManipulation

        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public ChanceLevelField TimeManipulationDropChance;

        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public TimeLevelField TimeManipulationPauseSize;

        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public TimeLevelField TimeManipulationDuration;

        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))] [Range(0, 1)]
        public float TimeManipulationScaleRate;

        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))] [MinValue(0)]
        public float TimeManipulationScaleSpeed;

        #endregion

        #region CashStorage

        [FoldoutGroup(nameof(BonusItemTypes.CashStorage))] [MinValue(1)]
        public float CashStoragePercentLimit;

        #endregion

        #region Efficiency

        [FoldoutGroup(nameof(BonusItemTypes.Efficiency))] [MinValue(0)]
        public int EfficiencyIncreaseLevelSize;

        #endregion

        #region MagnitudeField

        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeStartPause;

        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeSpeedToggle;

        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeStartSpeed;

        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeAccelerationSpeed;

        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeMaxSpeed;

        #endregion

        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false, ShowIndexLabels = false)]
        public InventoryItem[] InventoryItems = new InventoryItem[0];

        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false, ShowIndexLabels = false)]
        public BonusItem[] BonusItems = new BonusItem[0];

#if UNITY_EDITOR
        [Button]
        private void EditorLoadItems()
        {
            InventoryItems = Resources.LoadAll<InventoryItem>("");
        }

        [Button]
        private void EditorLoadBonuses()
        {
            BonusItems = Resources.LoadAll<BonusItem>("");
        }
#endif
    }
}