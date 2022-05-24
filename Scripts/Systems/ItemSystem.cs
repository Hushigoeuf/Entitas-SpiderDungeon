using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Инициализирует работу предметов.
    /// </summary>
    public sealed class ItemSystem : IInitializeSystem, IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly ItemSettings _itemSettings;
        private readonly IGroup<ConfigEntity> _itemGroup;
        private readonly List<ConfigEntity> _itemBuffer;

        private readonly bool _contentAlwaysIncluded;
        private bool _loadSkipped;

        public ItemSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _itemSettings = settings.ItemSettings;
            _itemGroup = contexts.config.GetGroup(ConfigMatcher.Item);
            _itemBuffer = new List<ConfigEntity>();

            _contentAlwaysIncluded = GameSettings.ITEM_ALWAYS_WORKING;
        }

        public void Initialize()
        {
            // Создает сущности под предметы инвентаря
            foreach (var item in _itemSettings.InventoryItems)
            {
                if (!_contentAlwaysIncluded && !item.IsWorking) continue;

                var e = CreateItemEntity(item);
                e.AddInventoryItemType(item.ItemType);
                e.AddLevel(item.Level, 0);
            }

            // Создает сущности под бонусные предметы
            foreach (var bonus in _itemSettings.BonusItems)
            {
                if (!_contentAlwaysIncluded && !bonus.IsWorking) continue;

                var e = CreateItemEntity(bonus);
                e.AddBonusItemType(bonus.BonusType);
            }
        }

        private ConfigEntity CreateItemEntity(LifetimeItem contentObject)
        {
            var e = _contexts.config.CreateEntity();
            {
                e.isItem = true;
                e.isControlled = true;
                e.AddCountParameter(contentObject.LifetimeParameter);
            }
            return e;
        }

        /// <summary>
        /// Вычисляет рабочее состояние всех инициированных предметов.
        /// </summary>
        public void Execute()
        {
            if (!_loadSkipped)
            {
                _itemGroup.GetEntities(_itemBuffer);
                _loadSkipped = _itemBuffer.Count == 0;
            }

            if (_loadSkipped) return;

            for (var i = 0; i < _itemBuffer.Count; i++)
            {
                if (!_itemBuffer[i].isControlled) continue;
                if (_itemBuffer[i].countParameter.Value.Value > 0 &&
                    _itemBuffer[i].countParameter.Value.Value > 0) continue;
                if (!_contentAlwaysIncluded)
                    _itemBuffer[i].isCleanup = true;
            }
        }
    }
}