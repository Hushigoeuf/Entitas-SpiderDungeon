using Entitas;

namespace GameEngine
{
    public abstract class GameEntitasBehaviour : EntitasBehaviour
    {
        protected bool IsMainConfig => _contexts.config.isMainConfig;
        protected ConfigEntity MainConfig => _contexts.config.mainConfigEntity;
        protected bool IsGameOver => IsMainConfig && MainConfig.isGameOver;

        protected virtual void Start()
        {
            if (IsMainConfig) MainConfig.OnComponentAdded += OnComponentAddedToMainConfig;
        }

        protected void OnComponentAddedToMainConfig(IEntity entity, int index, IComponent component)
        {
            if (!MainConfig.isGameOver) return;

            OnGameOver();

            MainConfig.OnComponentAdded -= OnComponentAddedToMainConfig;
        }

        protected virtual void OnGameOver()
        {
        }
    }
}