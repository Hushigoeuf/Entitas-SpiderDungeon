using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class ScoreSpawnSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly Contexts _contexts;
        private readonly ScoreSettings _scoreSettings;
        private readonly IPoolService _poolService;
        private readonly Transform _camera;

        public ScoreSpawnSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.environment)
        {
            _contexts = contexts;
            _scoreSettings = settings.ScoreSettings;
            _poolService = services.Pool;
            _camera = camera;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_SCORE &&
            _contexts.config.mainConfigEntity.delayCount.Value <= 0;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            foreach (var e in entities)
            {
                var score = _poolService.Spawn(e.pool.Value, _scoreSettings.Prefab);
                score.position = new Vector3(score.position.x, e.position.Value.y, score.position.z);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_camera);
                    eDestroy.AddTarget(score);
                    eDestroy.AddOffset(ScreenSettings.GetTopScreenPoint(5.12f));
                    eDestroy.AddPool(e.pool.Value);
                }
            }
        }
    }
}