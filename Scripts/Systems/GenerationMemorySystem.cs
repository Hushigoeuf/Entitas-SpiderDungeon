using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Заполняет GenerationMemory рандомизированной информацией.
    /// Эта информация показывает какие препятствия можно создать, где можно создать алмазы.
    /// GenerationMemory специальный компонент, который содержит такую информацию.
    /// И может использоваться другими системами.
    /// </summary>
    public sealed class GenerationMemorySystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private const int MIN_GATE_INDEX = 0;
        private const int MAX_GATE_INDEX = 3;
        private const int MAX_GATE_RANGE = 2;
        private const bool SAME_GATE_ENABLED = false;

        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly TrapPrefab[] _traps;
        private readonly TrapChecker[] _checkers;

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _mainConfigEntity;
        private int _genGateIndex = -1;
        private List<int> _genVerifiedIndexes = new List<int>();
        private List<int> _genRandomIndexes = new List<int>();
        private List<int> _genBuildingIndexes = new List<int>();
        private int[] _genBuildingPosition = new int[] {0, 0, 0, 0};

        public GenerationMemorySystem(Contexts contexts, Services services, Settings settings) :
            base(contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.Random;

            var count = 0;
            foreach (var t in settings.TrapSettings.Types) count += t.Prefabs.Length;

            _traps = new TrapPrefab[count];
            _checkers = new TrapChecker[count];
            var index = -1;
            foreach (var t in settings.TrapSettings.Types)
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
            _mainConfigEntity = _contexts.config.mainConfigEntity;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
            _contexts.environment.hasGenerationMemory;

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
            Step_0_StartGeneration();
            Step_1_GenerateGateIndex();
            Step_2_GenerateVerifiedIndexes();
            Step_3_GenerateRandomIndexes();
            Step_4_GenerateBuildingIndexes();
            Step_5_StopGeneration();
        }

        /// <summary>
        /// Сбрасывает последнюю информацию из памяти.
        /// </summary>
        private void Step_0_StartGeneration()
        {
            if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                _memoryEntity.generationMemory.PrefabList.Clear();

            for (var i = 0; i < _memoryEntity.generationMemory.DiamondFreeSpaces.Length; i++)
                _memoryEntity.generationMemory.DiamondFreeSpaces[i] = false;
        }

        /// <summary>
        /// Находит свободный целевой проход, который не будет затронут препятствием.
        /// Уровень имеет 4 прохода - разделение по горизонтали.
        /// </summary>
        private void Step_1_GenerateGateIndex()
        {
            var range = MAX_GATE_RANGE > 0 ? MAX_GATE_RANGE : !SAME_GATE_ENABLED ? 1 : 0;
            var min = _genGateIndex != -1 ? _genGateIndex - range : MIN_GATE_INDEX;
            if (min < MIN_GATE_INDEX) min = MIN_GATE_INDEX;
            var max = _genGateIndex != -1 ? _genGateIndex + range : MAX_GATE_INDEX;
            if (max > MAX_GATE_INDEX) max = MAX_GATE_INDEX;

            if (!SAME_GATE_ENABLED)
            {
                if (min == _genGateIndex) min++;
                else if (max == _genGateIndex) max--;
            }

            var requiredGateIndex = _genGateIndex;
            for (var i = 0; i < 128; i++)
            {
                requiredGateIndex = _randomService.Range(min, max, true);
                if (requiredGateIndex == _genGateIndex && !SAME_GATE_ENABLED) continue;
                break;
            }

            _genGateIndex = requiredGateIndex;
        }

        /// <summary>
        /// Заполняет целевой список проверенными препятствиями, которые соответствуют текущими условиями.
        /// </summary>
        private void Step_2_GenerateVerifiedIndexes()
        {
            for (var i = 0; i < _traps.Length; i++)
            {
                if (_traps[i].Position[_genGateIndex] == 1) continue;
                if (!_Step_2_Check(_checkers[i], true)) continue;
                if (!_traps[i].UseOnlyGlobalChecker && !_Step_2_Check(_traps[i].Checker, true)) continue;
                _genVerifiedIndexes.Add(i);
            }
        }

        private bool _Step_2_Check(TrapChecker checker, bool chanceIgnored)
        {
            if (_memoryEntity.index.Value < checker.StartAtIndex) return false;
            if (_mainConfigEntity.level.FirstLevel < checker.MinRegionIndex) return false;
            if (!chanceIgnored && !_randomService.IsChance(checker.Chance)) return false;
            return true;
        }

        /// <summary>
        /// Заполняет целевой список рандомизированными препятствиями на основе проверенного списка.
        /// </summary>
        private void Step_3_GenerateRandomIndexes()
        {
            for (var j = 0; j < 2; j++)
            {
                _randomService.Shuffle(ref _genVerifiedIndexes);
                for (var i = 0; i < _genVerifiedIndexes.Count; i++)
                {
                    var index = _genVerifiedIndexes[i];
                    var chance = _checkers[index].Chance;
                    if (!_traps[index].UseOnlyGlobalChecker)
                        chance = chance * _traps[index].Checker.Chance;
                    if (!_randomService.IsChance(chance)) continue;
                    _genRandomIndexes.Add(index);
                }
            }

            _randomService.Shuffle(ref _genVerifiedIndexes);
            for (var i = 0; i < _genVerifiedIndexes.Count; i++)
                _genRandomIndexes.Add(_genVerifiedIndexes[i]);
        }

        /// <summary>
        /// Заполняет целевой список итоговыми препятствиями, которые могут быть созданы в дальнейшем.
        /// </summary>
        private void Step_4_GenerateBuildingIndexes()
        {
            for (var i = 0; i < _genRandomIndexes.Count; i++)
            {
                var position = _traps[_genRandomIndexes[i]].Position;
                if (position[_genGateIndex] != 2) continue;
                _genBuildingIndexes.Add(_genRandomIndexes[i]);

                if (position[0] != 0) _genBuildingPosition[0] = position[0];
                if (position[1] != 0) _genBuildingPosition[1] = position[1];
                if (position[2] != 0) _genBuildingPosition[2] = position[2];
                if (position[3] != 0) _genBuildingPosition[3] = position[3];

                break;
            }

            for (var i = 0; i < _genRandomIndexes.Count; i++)
            {
                if (!_Step_4_Check(_genRandomIndexes[i])) continue;
                _genBuildingIndexes.Add(_genRandomIndexes[i]);

                var position = _traps[_genRandomIndexes[i]].Position;
                if (position[0] != 0) _genBuildingPosition[0] = position[0];
                if (position[1] != 0) _genBuildingPosition[1] = position[1];
                if (position[2] != 0) _genBuildingPosition[2] = position[2];
                if (position[3] != 0) _genBuildingPosition[3] = position[3];

                if (_memoryEntity.generationMemory.DecreaseDifficulty) break;
                if ((_genBuildingPosition[0] != 0 || _genGateIndex == 0) &&
                    (_genBuildingPosition[1] != 0 || _genGateIndex == 1) &&
                    (_genBuildingPosition[2] != 0 || _genGateIndex == 2) &&
                    (_genBuildingPosition[3] != 0 || _genGateIndex == 3))
                    break;
            }
        }

        /// <summary>
        /// Проверяет, чтобы целевое препятствие не мешало уже выбранным препятствиям.
        /// </summary>
        private bool _Step_4_Check(int index)
        {
            var position = _traps[index].Position;
            if (_genBuildingPosition[0] != 0 && position[0] != 0) return false;
            if (_genBuildingPosition[1] != 0 && position[1] != 0) return false;
            if (_genBuildingPosition[2] != 0 && position[2] != 0) return false;
            if (_genBuildingPosition[3] != 0 && position[3] != 0) return false;

            if (_genGateIndex != 0)
                if (_genBuildingPosition[_genGateIndex - 1] == 0 && position[_genGateIndex - 1] == 2)
                    return false;
            if (_genGateIndex != 3)
                if (_genBuildingPosition[_genGateIndex + 1] == 0 && position[_genGateIndex + 1] == 2)
                    return false;

            var totalGateCount = 0;
            if (_genBuildingPosition[0] == 2) totalGateCount++;
            if (_genBuildingPosition[1] == 2) totalGateCount++;
            if (_genBuildingPosition[2] == 2) totalGateCount++;
            if (_genBuildingPosition[3] == 2) totalGateCount++;

            var currentGateCount = 0;
            if (position[0] == 2) currentGateCount++;
            if (position[1] == 2) currentGateCount++;
            if (position[2] == 2) currentGateCount++;
            if (position[3] == 2) currentGateCount++;

            if (totalGateCount != 0 && currentGateCount != 0) return false;

            return true;
        }

        /// <summary>
        /// Заполняет данные финальной информацией.
        /// В том числе информацией о возможной позиции алмазов.
        /// </summary>
        private void Step_5_StopGeneration()
        {
            if (_mainConfigEntity.delayCount.Value > 0)
            {
                _mainConfigEntity.ReplaceDelayCount(_mainConfigEntity.delayCount.Value - 1);
                _genBuildingPosition[0] = 1;
                _genBuildingPosition[1] = 1;
                _genBuildingPosition[2] = 1;
                _genBuildingPosition[3] = 1;
            }
            else
            {
                _memoryEntity.ReplaceIndex(_memoryEntity.index.Value + 1);
                for (var i = 0; i < _genBuildingIndexes.Count; i++)
                {
                    _memoryEntity.generationMemory.PrefabList.Add(_traps[_genBuildingIndexes[i]].Prefab);
                    for (var j = 0; j < _traps[_genBuildingIndexes[i]].DiamondFreeSpaces.Length; j++)
                        if (_traps[_genBuildingIndexes[i]].DiamondFreeSpaces[j])
                            _memoryEntity.generationMemory.DiamondFreeSpaces[j] = true;
                }
            }

            _memoryEntity.environmentPosition.Set(_genBuildingPosition);

            _genVerifiedIndexes.Clear();
            _genRandomIndexes.Clear();
            _genBuildingIndexes.Clear();

            _genBuildingPosition[0] = 0;
            _genBuildingPosition[1] = 0;
            _genBuildingPosition[2] = 0;
            _genBuildingPosition[3] = 0;
        }
    }
}