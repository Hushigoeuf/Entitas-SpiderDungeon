using Entitas;

namespace GameEngine
{
    public sealed class PoolSystem : IInitializeSystem
    {
        private readonly IPoolService _poolService;
        private readonly string[] _poolIds;

        public PoolSystem(Services services, params string[] poolIds)
        {
            _poolService = services.PoolService;
            _poolIds = poolIds;
        }

        public void Initialize()
        {
            for (var i = 0; i < _poolIds.Length; i++) _poolService.Create(_poolIds[i]);
        }
    }
}