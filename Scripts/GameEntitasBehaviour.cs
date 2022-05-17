using Entitas;

namespace GameEngine
{
    public abstract class GameEntitasBehaviour : EntitasBehaviour
    {
        private ConfigEntity _mainConfigEntity;
        private bool _isGameOver;

        protected bool IsMainConfig => Contexts.config.isMainConfig;
        protected ConfigEntity MainConfigEntity => _mainConfigEntity;
        protected bool IsGameOver => IsMainConfig && MainConfigEntity.isGameOver;
        
        protected override void Start()
        {
            base.Start();

            if (Contexts.config.isMainConfig)
            {
                _mainConfigEntity = Contexts.config.mainConfigEntity;
                _mainConfigEntity.OnComponentAdded += _OnComponentAddedToMainConfigEntity;
                _mainConfigEntity.OnDestroyEntity += _OnDestroyMainConfigEntity;
            }
        }

        private void _OnComponentAddedToMainConfigEntity(IEntity entity, int index, IComponent component)
        {
            if (_mainConfigEntity.isGameOver && !_isGameOver)
            {
                _isGameOver = true;
                OnGameOver();
            }

            _mainConfigEntity.OnComponentAdded -= _OnComponentAddedToMainConfigEntity;
        }

        private void _OnDestroyMainConfigEntity(IEntity entity)
        {
            _mainConfigEntity.OnDestroyEntity -= _OnDestroyMainConfigEntity;
            _mainConfigEntity = null;
        }

        protected virtual void OnGameOver()
        {
        }
    }
}