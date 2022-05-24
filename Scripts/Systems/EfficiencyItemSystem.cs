using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Повышает уровень предметов если работает предмет на эффективность.
    /// </summary>
    public sealed class EfficiencyItemSystem : ReactiveSystem<ConfigEntity>
    {
        private readonly ItemSettings _settings;
        private readonly IGroup<ConfigEntity> _group;
        private readonly List<ConfigEntity> _buffer;

        public EfficiencyItemSystem(Contexts contexts, Settings settings) : base(contexts.config)
        {
            _settings = settings.ItemSettings;
            _group = contexts.config.GetGroup(ConfigMatcher.AllOf(ConfigMatcher.Item, ConfigMatcher.Level));
            _buffer = new List<ConfigEntity>();
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context) =>
            context.CreateCollector(ConfigMatcher.AllOf(ConfigMatcher.Item,
                ConfigMatcher.BonusItemType));

        protected override bool Filter(ConfigEntity entity) => entity.bonusItemType.Value == BonusItemTypes.Efficiency;

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;
            for (var i = 0; i < entities.Count; i++)
            {
                foreach (var e in _group.GetEntities(_buffer))
                    e.level.SecondLevel = _settings.EfficiencyIncreaseLevelSize;
                entities[i].OnDestroyEntity += OnDestroyItemEntity;
            }
        }

        private void OnDestroyItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyItemEntity;
            foreach (var e in _group.GetEntities(_buffer)) e.level.SecondLevel = 0;
        }
    }
}