using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Создает алмазы на основе GenerationMemory.
    /// </summary>
    public sealed class DiamondGenerationSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly IPoolService _poolService;
        private readonly TrapSettings _trapSettings;
        private readonly DiamondSettings _diamondSettings;
        private readonly GameObject[] _prefabs;
        private readonly float[] _chances;
        private readonly List<float> _positionBuffer = new List<float>();
        private readonly Transform _camera;

        private bool _differentDiamondIncluded;
        private EnvironmentEntity _memoryEntity;
        private int[] _memoryPosition;
        private int[] _verticalFreeSpaces;

        public DiamondGenerationSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.Random;
            _poolService = services.Pool;
            _trapSettings = settings.TrapSettings;
            _diamondSettings = settings.DiamondSettings;
            _camera = camera;

            _prefabs = new GameObject[_diamondSettings.OtherPrefabList.Length + 1];
            _chances = new float[_diamondSettings.OtherPrefabList.Length + 1];
            _prefabs[0] = _diamondSettings.DefaultPrefab;
            _chances[0] = 1f;
            var index = 0;
            foreach (var p in _diamondSettings.OtherPrefabList)
            {
                index++;
                _prefabs[index] = p.Prefab;
                _chances[index] = p.Chance;
            }

            _memoryPosition = new int[4];
            _verticalFreeSpaces = new int[4];
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.Item,
                         ConfigMatcher.BonusItemType)))
            {
                if (e.bonusItemType.Value != BonusItemTypes.DifferentDiamonds) continue;

                _differentDiamondIncluded = true;
                e.OnDestroyEntity += _OnDestroyDifferentDiamondEntity;
                break;
            }

            _memoryEntity = _contexts.environment.generationMemoryEntity;
            _memoryPosition = new int[_memoryEntity.environmentPosition.GetSize()];
        }

        private void _OnDestroyDifferentDiamondEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyDifferentDiamondEntity;
            _differentDiamondIncluded = false;
        }

        protected override ICollector<EnvironmentEntity> GetTrigger(IContext<EnvironmentEntity> context) =>
            context.CreateCollector(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.GenerationTask,
                EnvironmentMatcher.Pool,
                EnvironmentMatcher.Position));

        protected override bool Filter(EnvironmentEntity entity) =>
            entity.pool.Value == GameSettings.POOL_ID_ENVIRONMENT_TRAPS &&
            _contexts.environment.hasGenerationMemory &&
            _contexts.config.mainConfigEntity.delayCount.Value <= 0;

        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;

            _memoryEntity.environmentPosition.Fill(ref _memoryPosition);

            for (var i = 0; i < entities.Count; i++) Execute(entities[i]);
        }

        private void Execute(EnvironmentEntity entity)
        {
            var positionY = entity.position.Value.y;
            var distancePerDiamond = _diamondSettings.DistancePerDiamond;

            for (var i = 0; i < _verticalFreeSpaces.Length; i++)
            {
                _verticalFreeSpaces[i] = 0;
                if (_memoryPosition[i] == 0 || _memoryEntity.generationMemory.DiamondFreeSpaces[i])
                    _verticalFreeSpaces[i] = 1;
            }

            // Создает алмазы по вертикали если позволяют условия
            if (_verticalFreeSpaces[0] != 0 ||
                _verticalFreeSpaces[1] != 0 ||
                _verticalFreeSpaces[2] != 0 ||
                _verticalFreeSpaces[3] != 0)
            {
                var count = _randomService.Range(_diamondSettings.MinDropCountPerOnce,
                    _diamondSettings.MaxDropCountPerOnce,
                    true);
                var distancePerColumn = GameSettings.GENERATION_GATE_SIZE;

                if (_verticalFreeSpaces[0] != 0) _positionBuffer.Add(0 - distancePerColumn - distancePerColumn / 2);
                if (_verticalFreeSpaces[1] != 0) _positionBuffer.Add(0 - distancePerColumn / 2);
                if (_verticalFreeSpaces[2] != 0) _positionBuffer.Add(0 + distancePerColumn / 2);
                if (_verticalFreeSpaces[3] != 0) _positionBuffer.Add(0 + distancePerColumn + distancePerColumn / 2);

                if (_positionBuffer.Count == 1 || !_randomService.IsChance(_diamondSettings.SplitChance))
                {
                    var first = _positionBuffer[0];
                    _positionBuffer.Clear();
                    _positionBuffer.Add(first);
                }
                else
                {
                    count = _randomService.Range(_diamondSettings.MinDropCountPerSplit,
                        _diamondSettings.MaxDropCountPerSplit,
                        true);
                }

                positionY += count * distancePerDiamond / 2f - distancePerDiamond / 2;
                for (var pi = 0; pi < _positionBuffer.Count; pi++)
                for (var i = 0; i < count; i++)
                    CreateDiamond(_positionBuffer[pi], positionY - distancePerDiamond * i);

                _positionBuffer.Clear();
                return;
            }

            // Создает алмазы по горизонтали если позволяют условия
            if (_randomService.IsChance(_diamondSettings.HorizontalDropChance))
            {
                var count = _randomService.Range(_diamondSettings.MinDropCountForHorizontal,
                    _diamondSettings.MaxDropCountForHorizontal, true);
                var positionX = 0f - count * distancePerDiamond / 2f + distancePerDiamond / 2;
                positionY += _trapSettings.GenerationStepSize / 2;

                var doubleSplit = _diamondSettings.HorizontalSplitChance > 0 &&
                                  _randomService.IsChance(_diamondSettings.HorizontalSplitChance);
                for (var i = 0; i < count; i++)
                    if (doubleSplit)
                    {
                        CreateDiamond(positionX + distancePerDiamond * i, positionY + 1.5f);
                        CreateDiamond(positionX + distancePerDiamond * i, positionY - 1.5f);
                    }
                    else
                    {
                        CreateDiamond(positionX + distancePerDiamond * i, positionY);
                    }
            }
        }

        /// <summary>
        /// Создает алмаз в заданной позиции.
        /// </summary>
        private void CreateDiamond(float x, float y)
        {
            var targetPrefab = _diamondSettings.DefaultPrefab;
            if (_differentDiamondIncluded)
            {
                var randomPrefabs = _randomService.Choose(_prefabs, _chances);
                if (randomPrefabs.Length != 0) targetPrefab = randomPrefabs[0];
            }

            var diamond = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS, targetPrefab);
            diamond.position = new Vector3(x, y, diamond.position.z);

            var eDestroy = Contexts.sharedInstance.environment.CreateEntity();
            {
                eDestroy.isDestroy = true;
                eDestroy.AddTransform(_camera);
                eDestroy.AddTarget(diamond);
                eDestroy.AddOffset(ScreenSettings.GetTopScreenPoint(5.12f));
                eDestroy.AddPool(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS);
            }
        }
    }
}