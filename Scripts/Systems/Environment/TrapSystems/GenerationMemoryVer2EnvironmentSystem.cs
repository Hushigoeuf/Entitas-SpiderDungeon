/*using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationMemoryVer2EnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;

        private readonly TrapPrefab[] _prefabs;
        private readonly float[] _chances;
        private readonly TrapChecker[] _checkers;
        private int[] _indexes;

#if UNITY_EDITOR
        private readonly GameObject[] _debugPrefabList;
#endif

        private EnvironmentEntity _memoryEntity;
        private ConfigEntity _mainConfigEntity;

        // -------------------------------------------------------------------------------------------------------------

        public GenerationMemoryVer2EnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.RandomService;

            // Вычисляем кол-во префабов по всем типам.
            var count = 0;
            foreach (var t in data.TrapData.Types) count += t.Prefabs.Length;

            // Берем данные из настроек и "раскладываем" их по переменным.
            _prefabs = new TrapPrefab[count];
            _chances = new float[count];
            _checkers = new TrapChecker[count];
            _indexes = new int[count];
            var index = -1;
            foreach (var t in data.TrapData.Types)
            foreach (var p in t.Prefabs)
            {
                index++;
                _prefabs[index] = p;
                _chances[index] = t.Checker.Chance;
                if (!p.UseOnlyGlobalChecker)
                    _chances[index] = (_chances[index] + p.Checker.Chance) / 2;
                _checkers[index] = t.Checker;
                _indexes[index] = index;
            }

#if UNITY_EDITOR
            _debugPrefabList = data.TrapData.DebugPrefabList;
#endif
        }

        public void Initialize()
        {
            var eMemory = _contexts.environment.CreateEntity();
            {
                eMemory.AddGenerationMemory(new List<GameObject>(), 1f, 1);
                eMemory.AddEnvironmentPosition(0, 0, 0, 0);
                eMemory.AddIndex(-1);
            }

            _memoryEntity = _contexts.environment.generationMemoryEntity;
            _mainConfigEntity = _contexts.config.mainConfigEntity;
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

        // -------------------------------------------------------------------------------------------------------------

        // Сюда будут помещаться позиция последней генерации.
        private int[] _lastPosition = new[] {0, 0, 0, 0};

        // Здесь будет хранится позиция текущей генерации.
        private int[] _currentPosition = new[] {0, 0, 0, 0};

        // Здесь будут помещаться сгенерированные индексы по текущей генерации.
        private List<int> _generatedIndexes = new List<int>();

        // Здесь будут помещаться собранные индексы по текущей генерации.
        private List<int> _buildedIndexes = new List<int>();

        private void GenerateMemory(EnvironmentEntity entity)
        {
            _Step_0_InitializeGeneration();
            _Step_1_GenerateIndexes();
            _Step_2_BuildIndexes();

            for (var i = 0; i < _buildedIndexes.Count; i++)
                _memoryEntity.generationMemory.PrefabList.Add(_prefabs[_buildedIndexes[i]].Prefab);

#if UNITY_EDITOR
            if (_debugPrefabList.Length != 0)
            {
                if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                    _memoryEntity.generationMemory.PrefabList.Clear();
                _currentPosition = new int[] {2, 2, 2, 2};
                _memoryEntity.generationMemory.PrefabList.Add(_randomService.Choose(_debugPrefabList));
            }
#endif

            if (_mainConfigEntity.delayCount.Value > 0)
            {
                _mainConfigEntity.ReplaceDelayCount(_mainConfigEntity.delayCount.Value - 1);
                if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                    _memoryEntity.generationMemory.PrefabList.Clear();
                _currentPosition = new int[] {2, 2, 2, 2};
            }

            _memoryEntity.environmentPosition.Set(_currentPosition);
        }

        // 1. Проводим стартовые действия перед новой генерацией (сброс данных и т.п.)
        private void _Step_0_InitializeGeneration()
        {
            // Сбрасываем данные.
            if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                _memoryEntity.generationMemory.PrefabList.Clear();

            // Запоминаем позицию последней генерации.
            _memoryEntity.environmentPosition.Fill(ref _lastPosition);

            // Добавляем 1 к индексу генерации.
            _memoryEntity.ReplaceIndex(_memoryEntity.index.Value + 1);
        }

        // 2. Собираем массив индексов в случайном порядке, которые могут участвовать в генерации.
        private void _Step_1_GenerateIndexes()
        {
            _generatedIndexes.Clear();

            var leftGateEnabled = _lastPosition[0] == 1;
            var rightGateEnabled = _lastPosition[3] == 1;

            // Перемешиваем массив индексов.
            _randomService.Shuffle(ref _indexes);

            for (var i = 0; i < _indexes.Length; i++)
            {
                // Проверяем глобальный и локальный чекеры.
                if (!_Step_1_Check(_checkers[_indexes[i]], true)) continue;
                if (!_prefabs[_indexes[i]].UseOnlyGlobalChecker &&
                    !_Step_1_Check(_prefabs[_indexes[i]].Checker, true)) continue;

                // Сразу проверяем, чтобы входы не находились на противоположных концах карты относительно предыдущей генерации.
                var position = _prefabs[_indexes[i]].Position;
                if (!leftGateEnabled)
                    if (position[1] == 1 && position[2] == 1 && position[3] == 1)
                        continue;
                if (!rightGateEnabled)
                    if (position[0] == 1 && position[1] == 1 && position[2] == 1)
                        continue;

                _generatedIndexes.Add(_indexes[i]);
            }
        }

        // 1. Проверяет по чекеру возможность генерации.
        private bool _Step_1_Check(TrapChecker checker, bool chanceIgnored)
        {
            if (_memoryEntity.index.Value < checker.StartAtIndex) return false;
            if (_mainConfigEntity.level.FirstLevel < checker.MinRegionIndex) return false;
            if (!chanceIgnored && !_randomService.IsChance(checker.Chance)) return false;
            return true;
        }

        private void _Step_2_BuildIndexes()
        {
            Debug.Log("Generated indexes: " + _generatedIndexes.Count);
            
            _buildedIndexes.Clear();

            _currentPosition[0] = 0;
            _currentPosition[1] = 0;
            _currentPosition[2] = 0;
            _currentPosition[3] = 0;

            var finished = false;
            //while (!finished)
                for (var i = 0; i < _generatedIndexes.Count; i++)
                {
                    //if (!_randomService.IsChance(_chances[_generatedIndexes[i]])) continue;

                    var position = _prefabs[_generatedIndexes[i]].Position;
                    if (_currentPosition[0] != 0 && position[0] != 0) continue;
                    if (_currentPosition[1] != 0 && position[1] != 0) continue;
                    if (_currentPosition[2] != 0 && position[2] != 0) continue;
                    if (_currentPosition[3] != 0 && position[3] != 0) continue;

                    var currentGateCount = 0;
                    if (_currentPosition[0] == 2) currentGateCount++;
                    if (_currentPosition[1] == 2) currentGateCount++;
                    if (_currentPosition[2] == 2) currentGateCount++;
                    if (_currentPosition[3] == 2) currentGateCount++;

                    var positionGateCount = 0;
                    if (position[0] == 2) positionGateCount++;
                    if (position[1] == 2) positionGateCount++;
                    if (position[2] == 2) positionGateCount++;
                    if (position[3] == 2) positionGateCount++;

                    var freeGateCount = 0;
                    if (_currentPosition[0] + position[0] == 0) freeGateCount++;
                    if (_currentPosition[1] + position[1] == 0) freeGateCount++;
                    if (_currentPosition[2] + position[2] == 0) freeGateCount++;
                    if (_currentPosition[3] + position[3] == 0) freeGateCount++;

                    if (freeGateCount == 0 && currentGateCount + positionGateCount == 0) continue;

                    _currentPosition[0] = _currentPosition[0] + position[0];
                    _currentPosition[1] = _currentPosition[1] + position[1];
                    _currentPosition[2] = _currentPosition[2] + position[2];
                    _currentPosition[3] = _currentPosition[3] + position[3];

                    _buildedIndexes.Add(_generatedIndexes[i]);

                    if (freeGateCount == 0 || freeGateCount == 1 && currentGateCount + positionGateCount == 0)
                    {
                        finished = true;
                        break;
                    }
                }
        }
    }
}*/