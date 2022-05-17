using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class EfficiencyConfigSystem : ReactiveSystem<ConfigEntity>
    {
        private readonly ContentSettingsObject _contentSettings;
        private readonly IGroup<ConfigEntity> _group;
        private readonly List<ConfigEntity> _buffer;

        public EfficiencyConfigSystem(Contexts contexts, Services services, Data data) : base(contexts.config)
        {
            _contentSettings = data.ContentData;
            _group = contexts.config.GetGroup(ConfigMatcher.AllOf(ConfigMatcher.ContentObject, ConfigMatcher.Level));
            _buffer = new List<ConfigEntity>();
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context)
        {
            return context.CreateCollector(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.BonusBehaviourType));
        }

        protected override bool Filter(ConfigEntity entity)
        {
            return entity.bonusBehaviourType.Value == BonusBehaviourTypes.Efficiency;
        }

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;
            for (var i = 0; i < entities.Count; i++)
            {
                foreach (var e in _group.GetEntities(_buffer))
                    e.level.SecondLevel = _contentSettings.EfficiencyIncreaseLevelSize;
                entities[i].OnDestroyEntity += OnDestroyEfficiencyEntity;
            }
        }

        private void OnDestroyEfficiencyEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyEfficiencyEntity;
            foreach (var e in _group.GetEntities(_buffer)) e.level.SecondLevel = 0;
        }
    }
}