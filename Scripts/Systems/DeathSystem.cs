using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class DeathSystem : ReactiveSystem<FlightEntity>, IInitializeSystem, ICleanupSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly IGroup<FlightEntity> _characterGroup;

        private List<FlightEntity> _entities = new List<FlightEntity>();
        private List<FlightEntity> _buffer = new List<FlightEntity>();
        private ConfigEntity _statsEntity;

        public DeathSystem(Contexts contexts, Services services) : base(contexts.flight)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _characterGroup = contexts.flight.GetGroup(FlightMatcher.Character);
        }

        public void Initialize()
        {
            _statsEntity = _contexts.config.statConfigEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.Character,
                FlightMatcher.Death));

        protected override bool Filter(FlightEntity entity) => true;

        protected override void Execute(List<FlightEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
                Execute(entities[i]);
        }

        private void Execute(FlightEntity characterEntity)
        {
            var entities = _characterGroup.GetEntities(_buffer);
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i].hasDeath) continue;
                _entities.Add(entities[i]);
            }

            _entities.Sort((e0, e1) =>
            {
                return e0.index.Value < e1.index.Value ? -1 : e0.index.Value > e1.index.Value ? 1 : 0;
            });

            Transform target = null;
            for (var i = 0; i < _entities.Count; i++)
            {
                if (target == null)
                    target = _contexts.flight.guideOffsetEntity.transform.Value;

                _entities[i].character.FollowEntity.ReplaceTarget(target);
                _entities[i].character.RotationEntity.ReplaceTarget(target);
                _entities[i].ReplaceTarget(target);

                target = _entities[i].transform.Value;
            }

            _entities.Clear();
        }

        public void Cleanup()
        {
            foreach (var e in _characterGroup.GetEntities(_buffer))
            {
                if (!e.hasDeath) continue;

                if (e.hasTrapType) _statsEntity.deathCount.CategoryCounts[e.trapType.InstanceID]++;
                else _statsEntity.deathCount.NoCategoryCount++;

                e.character.FollowEntity.Destroy();
                e.character.RotationEntity.Destroy();

                _poolService.Despawn(GameSettings.POOL_ID_FLIGHT, e.transform.Value);

                e.Destroy();
            }
        }
    }
}