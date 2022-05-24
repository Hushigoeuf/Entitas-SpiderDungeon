using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Меняет спрайты стен на рандомизированные.
    /// </summary>
    public sealed class WallRandomSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly Contexts _contexts;
        private readonly WallSettings _wallSettings;
        private readonly Dictionary<int, WallRendererGroup> _groups = new Dictionary<int, WallRendererGroup>();

        public WallRandomSystem(Contexts contexts, Settings settings) : base(contexts.environment)
        {
            _contexts = contexts;
            _wallSettings = settings.WallSettings;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Target,
                EnvironmentMatcher.Pool));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_WALLS;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 1)
            {
                Execute(entities[0]);
                return;
            }

            for (var i = 0; i < entities.Count; i++)
                Execute(entities[i]);
        }

        private void Execute(EnvironmentEntity entity)
        {
            if (_groups.Count > 10) _groups.Clear();

            var groupID = entity.target.Value.GetInstanceID();
            var group = _groups.ContainsKey(groupID)
                ? _groups[groupID]
                : entity.target.Value.GetComponent<WallRendererGroup>();

            var sprites = _wallSettings.GetRandomWallSprites(_contexts.config.mainConfigEntity.level.FirstLevel, 3);
            group.FirstWallRenderer.sprite = sprites[0];
            group.SecondWallRenderer.sprite = sprites[1];
            group.ThirdWallRenderer.sprite = sprites[2];
        }
    }
}