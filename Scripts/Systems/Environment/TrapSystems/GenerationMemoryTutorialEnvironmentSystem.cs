using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationMemoryTutorialEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly TrapPrefab[] _traps;
        private readonly TrapChecker[] _checkers;

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _statsEntity;
        private int _lastTrapIndex = -1;

        public GenerationMemoryTutorialEnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;

            var count = 0;
            foreach (var t in data.TrapData.Types) count += t.Prefabs.Length;

            _traps = new TrapPrefab[count];
            _checkers = new TrapChecker[count];
            var index = -1;
            foreach (var t in data.TrapData.Types)
            foreach (var p in t.Prefabs)
            {
                index++;
                _traps[index] = p;
                _checkers[index] = t.Checker;
            }
        }

        public void Initialize()
        {
            var eMemory = _contexts.environment.CreateEntity();
            {
                eMemory.AddGenerationMemory(new List<GameObject>(), false, new bool[4]);
                eMemory.AddEnvironmentPosition(0, 0, 0, 0);
                eMemory.AddIndex(0);
            }

            _memoryEntity = _contexts.environment.generationMemoryEntity;
            _statsEntity = _contexts.config.statsConfigEntity;
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
            return entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
                   _contexts.environment.hasGenerationMemory;
        }

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;
            else if (entities.Count == 1) GenerateMemory(entities[0]);
            else
                for (var i = 0; i < entities.Count; i++)
                    GenerateMemory(entities[i]);
        }

        private void GenerateMemory(EnvironmentEntity entity)
        {
            if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                _memoryEntity.generationMemory.PrefabList.Clear();
            for (var i = 0; i < _memoryEntity.generationMemory.DiamondFreeSpaces.Length; i++)
                _memoryEntity.generationMemory.DiamondFreeSpaces[i] = false;

            _memoryEntity.environmentPosition.Set(1);

            for (var g = 0; g < 2; g++)
            {
                _lastTrapIndex++;
                if (_lastTrapIndex >= _traps.Length)
                    _lastTrapIndex = 0;

                for (var i = _lastTrapIndex; i < _traps.Length; i++)
                {
                    if (!Check(_checkers[i])) continue;
                    _memoryEntity.generationMemory.PrefabList.Add(_traps[i].Prefab);
                    _lastTrapIndex = i;
                    break;
                }

                if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                    break;
            }
        }

        private bool Check(TrapChecker checker)
        {
            //if (_memoryEntity.index.Value < checker.StartAtIndex) return false;
            if (_statsEntity.score.Value != checker.StartAtScore) return false;
            //if (_mainConfigEntity.level.FirstLevel < checker.MinRegionIndex) return false;
            return true;
        }
    }
}