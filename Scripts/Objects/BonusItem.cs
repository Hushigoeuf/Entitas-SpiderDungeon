using UnityEngine;

namespace GameEngine
{
    public enum BonusItemTypes
    {
        Unknown,
        CashStorage,
        DifferentDiamonds,
        Efficiency,
        MagnitudeField,
    }

    /// <summary>
    /// Класс для бонусных предметов.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(BonusItem))]
    public sealed class BonusItem : LifetimeItem
    {
        public BonusItemTypes BonusType = BonusItemTypes.Unknown;
    }
}