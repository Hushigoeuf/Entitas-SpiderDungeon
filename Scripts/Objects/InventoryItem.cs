using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public enum InventoryItemTypes
    {
        Unknown,
        AdditionalLife,
        Luck,
        Cocoon,
        Shield,
        Sonar,
        TimeManipulation,
    }

    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(InventoryItem))]
    public class InventoryItem : LifetimeItem
    {
        [Required] public CountParameter LevelParameter;
        [Required] public StatusParameter StatusParameter;
        public InventoryItemTypes ItemType = InventoryItemTypes.Unknown;

        public int Level
        {
            get => LevelParameter.Value;
            set => LevelParameter.Value = value;
        }

        public bool Status
        {
            get => StatusParameter.Value;
            set => StatusParameter.Value = value;
        }

        public override bool IsWorking => base.IsWorking && Status;

        public int NextLevelCost => (Level + 0) * 2;
    }
}