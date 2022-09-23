using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class ScoreGenerationSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly TrapSettings _trapSettings;
        private readonly Transform _camera;

        public ScoreGenerationSystem(Contexts contexts, Settings settings, Transform camera)
        {
            _contexts = contexts;
            _trapSettings = settings.TrapSettings;
            _camera = camera;
        }

        public void Initialize()
        {
            var size = _trapSettings.GenerationStepSize;
            var count = Mathf.CeilToInt(ScreenSettings.CurrentTransformHeight / 2f / size);
            var eGeneration = _contexts.environment.CreateEntity();
            {
                eGeneration.isGeneration = true;
                eGeneration.AddTransform(_camera);
                eGeneration.AddPosition(new Vector3(0, _camera.position.y + count * size + size / 2f, 0));
                eGeneration.AddOffset(count * size);
                eGeneration.AddLength(size);
                eGeneration.AddPool(GameSettings.POOL_ID_ENVIRONMENT_SCORE);
                eGeneration.AddIndex(0);
            }
        }
    }
}