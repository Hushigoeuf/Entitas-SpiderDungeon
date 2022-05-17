using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class WallEnvironmentSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly Contexts _contexts;
        private readonly WallData _wallData;
        private readonly Dictionary<int, WallBgSource> _sources = new Dictionary<int, WallBgSource>();

        public WallEnvironmentSystem(Contexts contexts, Data data) : base(contexts.environment)
        {
            _contexts = contexts;
            _wallData = data.WallData;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Target,
                EnvironmentMatcher.Pool));
        }

        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_WALLS;
        }


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
            if (_sources.Count > 10) _sources.Clear();
            var sourceId = entity.target.Value.GetInstanceID();
            var source = _sources.ContainsKey(sourceId)
                ? _sources[sourceId]
                : entity.target.Value.GetComponent<WallBgSource>();
            source.SetWallSprites(_wallData.GetRandomWallSprites(_contexts.config.mainConfigEntity.level.FirstLevel,
                WallBgSource.MAX_WALL_COUNT));
        }
    }
}