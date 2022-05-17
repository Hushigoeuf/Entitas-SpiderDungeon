/*using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationMemoryVer1EnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private const float GATE_SCORE_FOR_CLOSE_CELL = 1f;
        private const float GATE_SCORE_FOR_SUB_CELL = 1f;

        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly TrapData _trapData;

        private readonly TrapPrefab[] _prefabs;
        private readonly float[] _chances;
        private readonly TrapChecker[] _checkers;
        private int[] _indexes;

#if UNITY_EDITOR
        private readonly GameObject[] _debugPrefabList;
#endif

        private EnvironmentEntity _memoryEntity;
        private FlightEntity _guideEntity;
        private ConfigEntity _mainConfigEntity;

        // -------------------------------------------------------------------------------------------------------------

        public GenerationMemoryVer1EnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.RandomService;
            _trapData = data.TrapData;

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
                eMemory.AddGenerationMemory(new List<GameObject>(), 1f, 0);
                eMemory.AddEnvironmentPosition(0, 0, 0, 0);
                eMemory.AddIndex(-1);
            }

            _memoryEntity = _contexts.environment.generationMemoryEntity;
            _guideEntity = _contexts.flight.guideEntity;
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
            for (var i = 0; i < entities.Count; i++) GenerateMemory(entities[i]);
        }

        // -------------------------------------------------------------------------------------------------------------

        private int[] _lastPosition = new[] {0, 0, 0, 0};
        private int[] _currentPosition = new[] {0, 0, 0, 0};
        private int _lastEdgePosition;

        private void GenerateMemory(EnvironmentEntity entity)
        {
            // Сбрасываем предыдущие данные.
            {
                if (_memoryEntity.generationMemory.PrefabList.Count != 0)
                    _memoryEntity.generationMemory.PrefabList.Clear();
                for (var i = 0; i < _lastPosition.Length; i++) _lastPosition[i] = 0;
                for (var i = 0; i < _currentPosition.Length; i++) _currentPosition[i] = 0;
                _lastEdgePosition = 0;
            }

            _memoryEntity.environmentPosition.Fill(ref _lastPosition);

            // Добавляем 1 к индексу генерации.
            _memoryEntity.ReplaceIndex(_memoryEntity.index.Value + 1);

            // Вычисляем, находится ли игрок на краю карты.
            // Если да, то запрещаем создавать объекты с ед. выходом на противоположном краю карты.
            {
                if (_guideEntity.environmentPosition.X0 != 0) _lastEdgePosition = 1;
                else if (_guideEntity.environmentPosition.X3 != 0) _lastEdgePosition = -1;
            }

            // Перемешиваем массив индексов в случайном порядке и генерируем объекты.
            {
                var indexes = _randomService.Choose(_indexes, _chances, 8);

                for (var i = 0; i < indexes.Length; i++)
                    if (GenerateStepAndTryComplete(indexes[i]))
                        break;
            }

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

        private bool GenerateStepAndTryComplete(int index)
        {
            var position = _prefabs[index].Position;

            // Проверяем, есть ли свободное место для выбранного префаба.
            for (var i = 0; i < _currentPosition.Length; i++)
                if (_currentPosition[i] != 0 && position[i] != 0)
                    return false;

            // Проверяем чтобы предыдущий и текущий проходы не находились на противоположных краях уровня.
            {
                if (_lastEdgePosition != 0)
                    if (_currentPosition[1] == 1 && _currentPosition[2] == 1)
                        if (_lastEdgePosition == -1 && _currentPosition[0] == 1 ||
                            _lastEdgePosition == 1 && _currentPosition[3] == 1)
                            return false;
            }

            // Проверяем условия, которые были заданы для выбранного объекта и его типа.
            {
                if (!Check(_checkers[index], true)) return false;
                if (!_prefabs[index].UseOnlyGlobalChecker && !Check(_prefabs[index].Checker, true)) return false;
            }

            // Вычисляем очки за свободное место по текущим объектам.
            var currentGateScore = 0f;
            for (var i = 0; i < _currentPosition.Length; i++)
                if (_currentPosition[i] + position[i] != 1)
                    currentGateScore += _currentPosition[i] + position[i] == 0
                        ? GATE_SCORE_FOR_CLOSE_CELL
                        : GATE_SCORE_FOR_SUB_CELL;

            // Делаем первичную проверку выбранного объекта.
            {
                if (currentGateScore == 0) return false;
                if (currentGateScore < _memoryEntity.generationMemory.MinGateScore) return false;
            }

            // Запоминаем занятую позицию и добавляем объект в память генерации.
            for (var i = 0; i < _currentPosition.Length; i++)
                _currentPosition[i] = _currentPosition[i] + position[i];
            _memoryEntity.generationMemory.PrefabList.Add(_prefabs[index].Prefab);

            // Делаем вторичную проверку выбранного объекта.
            if (currentGateScore - _memoryEntity.generationMemory.MinGateScore > .5f) return false;

            return true;
        }

        private bool Check(TrapChecker checker, bool chanceIgnored)
        {
            if (!chanceIgnored && !_randomService.IsChance(checker.Chance)) return false;
            if (_memoryEntity.index.Value < checker.StartAtIndex) return false;
            return true;
        }
    }
}*/