using System;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class InitializeWallEnvironmentSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly ICameraService _cameraService;
        private readonly IPoolService _poolService;
        private readonly WallData _data;

        public InitializeWallEnvironmentSystem(Contexts contexts, Services services, WallData data)
        {
            _contexts = contexts;
            _cameraService = services.CameraService;
            _poolService = services.PoolService;
            _data = data;
        }

        public void Initialize()
        {
            // Creating start walls.
            {
                void CreateDestroyEntity(Transform target)
                {
                    var eDestroy = _contexts.environment.CreateEntity();
                    {
                        eDestroy.isDestroy = true;
                        eDestroy.AddTransform(_cameraService.Container);
                        eDestroy.AddTarget(target);
                        eDestroy.AddOffset(GameSettings.GetTopTransformPoint(_data.PixelsPerWall.ToUnits()));
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

                var middle = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _data.WallPrefab);
                {
                    middle.position = new Vector3(
                        middle.position.x,
                        _cameraService.Container.position.y,
                        middle.position.z);
                    CreateDestroyEntity(middle);
                    CreateTaskEntity(middle);
                }

                var count = Mathf.CeilToInt(GameSettings.TransformHeight / 2f / _data.PixelsPerWall.ToUnits());
                for (var i = 0; i < count; i++)
                {
                    var wall = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _data.WallPrefab);
                    {
                        wall.position = new Vector3(
                            middle.position.x,
                            _data.PixelsPerWall.ToUnits() * (i + 1),
                            middle.position.z);
                        CreateDestroyEntity(wall);
                        CreateTaskEntity(wall);
                    }
                }

                for (var i = 0; i < count; i++)
                {
                    var wall = _poolService.Spawn(GameSettings.POOL_ID_ENVIRONMENT_WALLS, _data.WallPrefab);
                    {
                        wall.position = new Vector3(
                            middle.position.x,
                            -_data.PixelsPerWall.ToUnits() * (i + 1),
                            middle.position.z);
                        CreateDestroyEntity(wall);
                        CreateTaskEntity(wall);
                    }
                }
            }

            // Creating generation for walls.
            {
                var size = _data.PixelsPerWall.ToUnits();
                var count = Mathf.CeilToInt(GameSettings.TransformHeight / 2f / size);
                
                var eGeneration = _contexts.environment.CreateEntity();
                {
                    eGeneration.isGeneration = true;
                    eGeneration.AddTransform(_cameraService.Container);
                    eGeneration.AddPosition(
                        new Vector3(0, _cameraService.Container.position.y + count * size, 0));
                    eGeneration.AddOffset(count * size);
                    eGeneration.AddLength(size);
                    eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_WALLS);
                    eGeneration.AddIndex(0);
                }
            }
        }
    }
}