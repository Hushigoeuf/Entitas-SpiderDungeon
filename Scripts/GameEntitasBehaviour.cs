using Entitas;

namespace GameEngine
{
    public abstract class GameEntitasBehaviour : EntitasBehaviour
    {
        /// Создана ли главная сущность с параметрами игры
        protected bool IsMainConfig => _contexts.config.isMainConfig;

        /// Возвращает главную сущность с параметрами игры
        protected ConfigEntity MainConfig => _contexts.config.mainConfigEntity;

        /// Является ли игра проигранной на данный момент
        protected bool IsGameOver => IsMainConfig && MainConfig.isGameOver;

        protected virtual void Start()
        {
            if (IsMainConfig) MainConfig.OnComponentAdded += OnComponentAddedToMainConfig;
        }

        /// <summary>
        /// Вылавливает статус проигрыша, что вызвать соответствующий метод.
        /// </summary>
        protected virtual void OnComponentAddedToMainConfig(IEntity entity, int index, IComponent component)
        {
            if (!MainConfig.isGameOver) return;

            OnGameOver();

            MainConfig.OnComponentAdded -= OnComponentAddedToMainConfig;
        }

        /// <summary>
        /// Вызывает единожды во время проигрыша.
        /// </summary>
        protected virtual void OnGameOver()
        {
        }
    }
}