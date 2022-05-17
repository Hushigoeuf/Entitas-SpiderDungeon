using System.Collections.Generic;
using Doozy.Engine;
using Entitas;

namespace GameEngine
{
    public sealed class GameOverConfigSystem : ReactiveSystem<ConfigEntity>
    {
        private readonly Contexts _contexts;

        private readonly IGroup<ConfigEntity> _contentObjectGroup;
        private readonly List<ConfigEntity> _contentObjectBuffer;

        public GameOverConfigSystem(Contexts contexts) : base(contexts.config)
        {
            _contexts = contexts;
            _contentObjectGroup = contexts.config.GetGroup(ConfigMatcher.ContentObject);
            _contentObjectBuffer = new List<ConfigEntity>();
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context)
        {
            return context.CreateCollector(ConfigMatcher.AllOf(
                ConfigMatcher.MainConfig,
                ConfigMatcher.GameOver));
        }

        protected override bool Filter(ConfigEntity entity)
        {
            return entity.isGameOver;
        }

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;

            _contentObjectGroup.GetEntities(_contentObjectBuffer);
            for (var i = 0; i < _contentObjectBuffer.Count; i++) _contentObjectBuffer[i].isCleanup = true;

            if (GameSettings.PlayCount == 0)
                GameEventMessage.SendEvent("FirstGameOver");
            else GameEventMessage.SendEvent("GameOver");
        }
    }
}