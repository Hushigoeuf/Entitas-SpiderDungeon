using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class GenerationDiamondEnvironmentSystem : ReactiveSystem<EnvironmentEntity>, IInitializeSystem
    {
        // 0 - empty, 1 - up, 2 - bottom, 3 - one, 4 - two
        /*private readonly int[,] _diamondPlaceScheme = new int[,]
        {
            {4, 0, 0, 4, 0, 0, 4},
            {4, 0, 0, 4, 0, 0, 4},
            {0, 0, 0, 0, 4, 0, 4},
            {4, 0, 4, 0, 0, 0, 0},
            {3, 0, 3, 0, 3, 0, 3},
            {3, 0, 3, 0, 3, 0, 3},
            {3, 0, 3, 0, 3, 0, 3},
            {0, 4, 0, 0, 0, 4, 0},
        };*/

        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly IPoolService _poolService;
        private readonly ICameraService _cameraService;
        private readonly TrapData _trapData;
        private readonly DiamondData _diamondData;

        private readonly GameObject[] _prefabs;
        private readonly float[] _chances;
        private readonly List<float> _positionBuffer = new List<float>();
        private bool _differentDiamondIncluded;
        private EnvironmentEntity _memoryEntity;
        private int[] _memoryPosition;
        private int[] _verticalFreeSpaces;

        public GenerationDiamondEnvironmentSystem(Contexts contexts, Services services, Data data) : base(
            contexts.environment)
        {
            _contexts = contexts;
            _randomService = services.RandomService;
            _poolService = services.PoolService;
            _cameraService = services.CameraService;
            _trapData = data.TrapData;
            _diamondData = data.DiamondData;

            _prefabs = new GameObject[_diamondData.OtherPrefabList.Length + 1];
            _chances = new float[_diamondData.OtherPrefabList.Length + 1];
            _prefabs[0] = _diamondData.DefaultPrefab;
            _chances[0] = 1f;
            var index = 0;
            foreach (var p in _diamondData.OtherPrefabList)
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
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.BonusBehaviourType)))
            {
                if (e.bonusBehaviourType.Value != BonusBehaviourTypes.DifferentDiamonds) continue;
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
                   _contexts.environment.hasGenerationMemory &&
                   _contexts.config.mainConfigEntity.delayCount.Value <= 0;
        }


        protected override void Execute(List<EnvironmentEntity> entities)
        {
            if (entities.Count == 0) return;

            _memoryEntity.environmentPosition.Fill(ref _memoryPosition);

            for (var i = 0; i < entities.Count; i++) Execute(entities[i]);
        }

        private void Execute(EnvironmentEntity entity)
        {
            var positionY = entity.position.Value.y;
            var distancePerDiamond = _diamondData.DistancePerDiamond;

            for (var i = 0; i < _verticalFreeSpaces.Length; i++)
            {
                _verticalFreeSpaces[i] = 0;
                if (_memoryPosition[i] == 0 || _memoryEntity.generationMemory.DiamondFreeSpaces[i])
                    _verticalFreeSpaces[i] = 1;
            }
            
            if (_verticalFreeSpaces[0] != 0 ||
                _verticalFreeSpaces[1] != 0 ||
                _verticalFreeSpaces[2] != 0 ||
                _verticalFreeSpaces[3] != 0)
            {
                var count = _randomService.Range(_diamondData.MinDropCountPerOnce, _diamondData.MaxDropCountPerOnce,
                    true);
                var distancePerColumn = GameSettings.LEVEL_PLACE_SIZE;

                if (_verticalFreeSpaces[0] != 0) _positionBuffer.Add(0 - distancePerColumn - distancePerColumn / 2);
                if (_verticalFreeSpaces[1] != 0) _positionBuffer.Add(0 - distancePerColumn / 2);
                if (_verticalFreeSpaces[2] != 0) _positionBuffer.Add(0 + distancePerColumn / 2);
                if (_verticalFreeSpaces[3] != 0) _positionBuffer.Add(0 + distancePerColumn + distancePerColumn / 2);

                if (_positionBuffer.Count == 1 || !_randomService.IsChance(_diamondData.SplitChance))
                {
                    var first = _positionBuffer[0];
                    _positionBuffer.Clear();
                    _positionBuffer.Add(first);
                }
                else
                {
                    count = _randomService.Range(_diamondData.MinDropCountPerSplit, _diamondData.MaxDropCountPerSplit,
                        true);
                }

                positionY += count * distancePerDiamond / 2f - distancePerDiamond / 2;
                for (var pi = 0; pi < _positionBuffer.Count; pi++)
                for (var i = 0; i < count; i++)
                    CreateDiamond(_positionBuffer[pi], positionY - distancePerDiamond * i);

                _positionBuffer.Clear();
                return;
            }

            if (_randomService.IsChance(_diamondData.HorizontalDropChance))
            {
                /*var randomSchemeId = _randomService.Index(_diamondPlaceScheme.GetLength(0));
                var posX = 0f - _distancePerDiamond - _distancePerDiamond / 2;
                var posY = positionY + _trapData.GenerationStepSize / 2;
                for (var i = 0; i < _diamondPlaceScheme.GetLength(1); i++)
                {
                    var randomSchemeValue = _diamondPlaceScheme[randomSchemeId, i];
                    if (randomSchemeValue == 0) continue;
                    if (randomSchemeValue == 1 || randomSchemeValue == 4)
                        CreateDiamond(posX + _distancePerDiamond / 2 * i, posY + _distancePerDiamond / 2);
                    if (randomSchemeValue == 2 || randomSchemeValue == 4)
                        CreateDiamond(posX + _distancePerDiamond / 2 * i, posY - _distancePerDiamond / 2);
                    if (randomSchemeValue == 3)
                        CreateDiamond(posX + _distancePerDiamond / 2 * i, posY);
                }

                return;*/
                var count = _randomService.Range(_diamondData.MinDropCountForHorizontal,
                    _diamondData.MaxDropCountForHorizontal, true);
                var positionX = 0f - count * distancePerDiamond / 2f + distancePerDiamond / 2;
                positionY += _trapData.GenerationStepSize / 2;

                var doubleSplit = _diamondData.HorizontalSplitChance > 0 &&
                                  _randomService.IsChance(_diamondData.HorizontalSplitChance);
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

        private void CreateDiamond(float x, float y)
        {
            var targetPrefab = _diamondData.DefaultPrefab;
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
                eDestroy.AddTransform(_cameraService.Container);
                eDestroy.AddTarget(diamond);
                eDestroy.AddOffset(GameSettings.GetTopTransformPoint(5.12f));
                eDestroy.AddPool(GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS);
            }
        }
    }
}