using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class FinishOnGameOverSystem : ReactiveSystem<ConfigEntity>
    {
        private readonly IGroup<ConfigEntity> _itemGroup;
        private readonly List<ConfigEntity> _itemBuffer;

        public FinishOnGameOverSystem(Contexts contexts) : base(contexts.config)
        {
            _itemGroup = contexts.config.GetGroup(ConfigMatcher.Item);
            _itemBuffer = new List<ConfigEntity>();
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context) =>
            context.CreateCollector(ConfigMatcher.AllOf(
                ConfigMatcher.MainConfig,
                ConfigMatcher.GameOver));

        protected override bool Filter(ConfigEntity entity) => entity.isGameOver;

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;

            _itemGroup.GetEntities(_itemBuffer);
            for (var i = 0; i < _itemBuffer.Count; i++)
                _itemBuffer[i].isCleanup = true;

            // if (GameSettings.PlayCount == 0)
            //     GameEventMessage.SendEvent("FirstGameOver");
            // else GameEventMessage.SendEvent("GameOver");
        }
    }
}