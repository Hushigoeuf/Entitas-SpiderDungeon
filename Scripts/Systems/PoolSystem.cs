using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Система инициализирует пулы объектов с заданными ID.
    /// </summary>
    public class PoolSystem : IInitializeSystem
    {
        protected IPoolService _poolService;
        protected string[] _poolIds;

        public PoolSystem(Services services, params string[] poolIds)
        {
            _poolService = services.Pool;
            _poolIds = poolIds;
        }

        public virtual void Initialize()
        {
            for (var i = 0; i < _poolIds.Length; i++)
                _poolService.Create(_poolIds[i]);
        }
    }
}