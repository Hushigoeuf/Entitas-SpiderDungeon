using Entitas;
using UnityEngine;

namespace GameEngine
{
    public sealed class InitializeScoreEnvironmentSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly ICameraService _cameraService;
        private readonly TrapData _trapData;

        public InitializeScoreEnvironmentSystem(Contexts contexts, Services services, Data data)
        {
            _contexts = contexts;
            _cameraService = services.CameraService;
            _trapData = data.TrapData;
        }

        public void Initialize()
        {
            var size = _trapData.GenerationStepSize;
            var count = Mathf.CeilToInt(GameSettings.TransformHeight / 2f / size);
            var eGeneration = _contexts.environment.CreateEntity();
            {
                eGeneration.isGeneration = true;
                eGeneration.AddTransform(_cameraService.Container);
                eGeneration.AddPosition(new Vector3(0,
                    _cameraService.Container.position.y + count * size + size / 2f, 0));
                eGeneration.AddOffset(count * size);
                eGeneration.AddLength(size);
                eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_SCORE);
                eGeneration.AddIndex(0);
            }
        }
    }
}