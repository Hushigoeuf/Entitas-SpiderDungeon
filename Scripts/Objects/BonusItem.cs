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

    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(BonusItem))]
    public class BonusItem : LifetimeItem
    {
        public BonusItemTypes BonusType = BonusItemTypes.Unknown;
    }
}