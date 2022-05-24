using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки параметров предметов.
    /// Так же содержит список всех предметов инвентаря и бонусных.
    /// 
    /// P.S. Такое скопление в одном месте лучше не делать, но можно управлять всем в одном.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(ItemSettings))]
    public sealed class ItemSettings : ScriptableObject
    {
        /// Кол-во жизней, который выпадают на старте
        [FoldoutGroup(nameof(InventoryItemTypes.AdditionalLife))]
        public CountLevelField LifeDropCount;

        /// Шанс выпадения удачи на снижение сложности препятствия
        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public ChanceLevelField LuckDropChance;

        /// Длительность паузы с последнего снижения сложности
        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public CountLevelField LuckDropPause;

        /// Сколько скорости будет сброшено если соответствующий предмет экипирован
        [FoldoutGroup(nameof(InventoryItemTypes.Luck))]
        public TimeLevelField LuckDropSpeed;

        /// Префаб кокона, который будет создан во время генерации
        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))] [Required]
        public GameObject CocoonPrefab;

        /// Шанс выпадения кокона
        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public ChanceLevelField CocoonDropChance;

        /// С какой итерации генератора начать спавн коконов
        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))] [MinValue(0)]
        public int CocoonPauseAfterStart;

        /// Сколько итераций генератора надо пропустить, чтобы снова создать кокон
        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public CountLevelField CocoonDropPause;

        /// Кол-во жизней, которые добавляет кокон
        [FoldoutGroup(nameof(InventoryItemTypes.Cocoon))]
        public CountLevelField CocoonDropLife;

        /// Шанс выпадения защитного поля
        [FoldoutGroup(nameof(InventoryItemTypes.Shield))]
        public ChanceLevelField ShieldDropChance;

        /// Шанс выпадения анализа для обнаружения препятствий
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public ChanceLevelField SonarDropChance;

        /// Кол-во проходов, которые будут участвовать в анализе за раз
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public CountLevelField SonarDropCount;

        /// Временная пауза с последнего анализа
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))]
        public TimeLevelField SonarDropPause;

        /// Позиция с которой начинается анализ (начиная от самой верхней точки экрана)
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(0)]
        public float SonarOffset;

        /// Длительность появления и исчезновения объекта при отображении
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(.1f)]
        public float SonarAnimationDuration;

        /// Пауза между появлением и исчезновением
        [FoldoutGroup(nameof(InventoryItemTypes.Sonar))] [MinValue(.1f)]
        public float SonarAnimationPause;

        /// Шанс замедлить время (после того как один из персонажей оказался в опасной ситуации)
        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public ChanceLevelField TimeManipulationDropChance;

        /// Длительность паузы с последнего спавна кокона
        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public TimeLevelField TimeManipulationPauseSize;

        /// Длительность замедления времени
        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))]
        public TimeLevelField TimeManipulationDuration;

        /// Уровень замедления времени (0 - игра остановлена, 1 - нормальное состояние)
        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))] [Range(0, 1)]
        public float TimeManipulationScaleRate;

        /// Скорость перехода в целевое состояние и из него
        [FoldoutGroup(nameof(InventoryItemTypes.TimeManipulation))] [MinValue(0)]
        public float TimeManipulationScaleSpeed;

        /// Кол-во алмазов, которые будет сохранено (в процентах)
        /// Шанс сохранить алмазы так же зависит от их кол-ва
        [FoldoutGroup(nameof(BonusItemTypes.CashStorage))] [MinValue(1)]
        public float CashStoragePercentLimit;

        /// Кол-во уровней, которое будет добавляться предметам инвентаря за получение бонуса на эффективность
        [FoldoutGroup(nameof(BonusItemTypes.Efficiency))] [MinValue(0)]
        public int EfficiencyIncreaseLevelSize;

        /// Время до начала создания магнитного поля
        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeStartPause;

        /// Скорость появления и исчезновения магнитного поля
        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeSpeedToggle;

        /// Стартовая скорость притяжения магнитного поля
        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeStartSpeed;

        /// Ускорение притяжения магнитного поля
        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeAccelerationSpeed;

        /// Максимальная скорость притяжения магнитного поля
        [FoldoutGroup(nameof(BonusItemTypes.MagnitudeField))] [MinValue(0)]
        public float MagnitudeMaxSpeed;

        /// Список предметов инвентаря (отсортированных в том виде в котором они будут отображаться)
        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowPaging = false, ShowIndexLabels = false)]
        public InventoryItem[] InventoryItems = new InventoryItem[0];

        /// Список бонусных предметов (отсортированных в том виде в котором они будут отображаться)
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