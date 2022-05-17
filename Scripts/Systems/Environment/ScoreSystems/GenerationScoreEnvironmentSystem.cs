using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationScoreEnvironmentSystem : ReactiveSystem<EnvironmentEntity>
    {
        private readonly Contexts _contexts;
        private readonly ScoreData _scoreData;
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;
        
        public GenerationScoreEnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _scoreData = data.ScoreData;
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context)
        {
            return context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));
        }


        protected override bool Filter(EnvironmentEntity entity)
        {
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_SCORE &&
                _contexts.config.mainConfigEntity.delayCount.Value <= 0;
        }


        protected override void Execute(List<EnvironmentEntity> entities)
        {
            foreach (var e in entities)
            {
                var score = _poolService.Spawn(e.pool.Value, _scoreData.Prefab);
                score.position = new Vector3(score.position.x, e.position.Value.y, score.position.z);

                var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
                {
                    eDestroy.isDestroy = true;
                    eDestroy.AddTransform(_cameraService.Container);
                    eDestroy.AddTarget(score);
                    eDestroy.AddOffset(GameSettings.GetTopTransformPoint(5.12f));
                    eDestroy.AddPool(e.pool.Value);
                }
            }
        }
    }
}