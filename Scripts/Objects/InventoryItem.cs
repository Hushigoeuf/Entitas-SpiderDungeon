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

    /// <summary>
    /// Класс для предметов инвентаря.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(InventoryItem))]
    public sealed class InventoryItem : LifetimeItem
    {
        [Required] public CountParameter LevelParameter;
        [Required] public StatusParameter StatusParameter;

        /// Тип предмета
        public InventoryItemTypes ItemType = InventoryItemTypes.Unknown;

        /// Уровень предмета
        public int Level
        {
            get => LevelParameter.Value;
            set => LevelParameter.Value = value;
        }

        /// Экипирован ли данный предмет в инвентаре
        public bool Status
        {
            get => StatusParameter.Value;
            set => StatusParameter.Value = value;
        }

        public override bool IsWorking => base.IsWorking && Status;

        /// Стоимость улучшения и перехода на следующий уровень
        public int NextLevelCost => (Level + 0) * 2;
    }
}