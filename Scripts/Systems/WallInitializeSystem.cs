using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Создает стартовые стены и инициализирует дальнейшую генерацию.
    /// </summary>
    public sealed class WallInitializeSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IPoolService _poolService;
        private readonly WallSettings _settings;
        private readonly Transform _camera;
        private readonly float _unitsPerWall;

        public WallInitializeSystem(Contexts contexts, Services services, Settings settings,
            Transform camera)
        {
            _contexts = contexts;
            _poolService = services.Pool;
            _settings = settings.WallSettings;
            _camera = camera;
            _unitsPerWall = ScreenSettings.PixelsToUnits(_settings.PixelsPerWall);
        }

        public void Initialize()
        {
            {
                void CreateDestroyEntity(Transform target)
                {
                    var eDestroy = _contexts.environment.CreateEntity();
                    {
                        eDestroy.isDestroy = true;
                        eDestroy.AddTransform(_camera);
                        eDestroy.AddTarget(target);
                        eDestroy.AddOffset(ScreenSettings.GetTopScreenPoint(_unitsPerWall));
                        eDestroy.AddPool(GameSettings.POOL_ID_ENVIRONMENT_WALLS);
                    }
                }

                void CreateTaskEntity(Transform target)
                {
                    var eTask = _contexts.environment.CreateEntity();
                    {
                        eTask.isGenerationTask = true;
                        eTask.AddTarget(target);
                        eTask.AddPool(GameSettings.POOL_ID_ENVIRONMENT_WALLS);
                        eTask.isCleanup = true;
                    }
                }

                var middle = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _settings.WallPrefab);
                {
                    middle.position = new Vector3(middle.position.x, _camera.position.y, middle.position.z);
                    CreateDestroyEntity(middle);
                    CreateTaskEntity(middle);
                }

                var count = Mathf.CeilToInt(ScreenSettings.CurrentTransformHeight / 2f / _unitsPerWall);
                for (var i = 0; i < count; i++)
                {
                    var wall = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _settings.WallPrefab);
                    {
                        wall.position = new Vector3(middle.position.x, _unitsPerWall * (i + 1), middle.position.z);
                        CreateDestroyEntity(wall);
                        CreateTaskEntity(wall);
                    }
                }

                for (var i = 0; i < count; i++)
                {
                    var wall = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _settings.WallPrefab);
                    {
                        wall.position = new Vector3(middle.position.x, -_unitsPerWall * (i + 1), middle.position.z);
                        CreateDestroyEntity(wall);
                        CreateTaskEntity(wall);
                    }
                }
            }

            // Инициализирует генерацию стен в дальнейшем
            {
                var count = Mathf.CeilToInt(ScreenSettings.CurrentTransformHeight / 2f / _unitsPerWall);

                var eGeneration = _contexts.environment.CreateEntity();
                {
                    eGeneration.isGeneration = true;
                    eGeneration.AddTransform(_camera);
                    eGeneration.AddPosition(new Vector3(0, _camera.position.y + count * _unitsPerWall, 0));
                    eGeneration.AddOffset(count * _unitsPerWall);
                    eGeneration.AddLength(_unitsPerWall);
                    eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_WALLS);
                    eGeneration.AddIndex(0);
                }
            }
        }
    }
}