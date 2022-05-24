using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Создает следы крови и остантков после гибели персонажа.
    /// </summary>
    public sealed class BloodSpawnSystem : ReactiveSystem<FlightEntity>
    {
        private readonly IPoolService _poolService;
        private readonly BloodSettings _bloodSettings;
        private readonly Transform _camera;

        private Dictionary<int, List<BloodPrefab>> _prefabs = new Dictionary<int, List<BloodPrefab>>();

        public BloodSpawnSystem(Contexts contexts, Services services, Settings settings,
            Transform camera) : base(contexts.flight)
        {
            _poolService = services.Pool;
            _bloodSettings = settings.BloodSettings;
            _camera = camera;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.AllOf(
                FlightMatcher.Character,
                FlightMatcher.Death));

        protected override bool Filter(FlightEntity entity) => entity.hasDeath;

        protected override void Execute(List<FlightEntity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
                Execute(entities[i]);
        }

        private void Execute(FlightEntity character)
        {
            if (!_prefabs.ContainsKey(character.death.ObstacleTypeIndex))
                _prefabs.Add(character.death.ObstacleTypeIndex,
                    _bloodSettings.GetPrefabs(character.death.ObstacleTypeIndex));

            foreach (var prefab in _prefabs[character.death.ObstacleTypeIndex])
            {
                var blood = _poolService.Spawn(GameSettings.POOL_ID_FLIGHT_BLOODS, prefab.Prefab);
                blood.transform.position = new Vector3(
                    prefab.IgnorePosition ? blood.transform.position.x : character.transform.Value.position.x,
                    character.transform.Value.position.y, blood.transform.position.z);
                if (!prefab.IgnoreRotation)
                    blood.eulerAngles = new Vector3(blood.eulerAngles.x, blood.eulerAngles.y,
                        character.transform.Value.eulerAngles.z + prefab.OffsetRotation);

                // Отправляем созданный объект в очередь на уничтожение
                var destroyEntity = Contexts.sharedInstance.environment.CreateEntity();
                {
                    destroyEntity.isDestroy = true;
                    destroyEntity.AddTransform(_camera);
                    destroyEntity.AddTarget(blood);
                    destroyEntity.AddOffset(ScreenSettings.GetTopScreenPoint(5.12f));
                    destroyEntity.AddPool(GameSettings.POOL_ID_FLIGHT_BLOODS);
                }
            }
        }
    }
}