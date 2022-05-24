using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// ECS-менеджер для основной сцены игры.
    /// </summary>
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(GameEntitasManager))]
    public class GameEntitasManager : EntitasManager<GameEntitasManager>
    {
        [Required] public Transform TargetCamera;
        [MinValue(0)] public int SkipStepCount;

        [FoldoutGroup(nameof(Settings))] [Required]
        public FlightSettings FlightSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public TrapSettings TrapSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public WallSettings WallSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public ScoreSettings ScoreSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public BloodSettings BloodSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public DiamondSettings DiamondSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public ItemSettings ItemSettings;

        [FoldoutGroup(nameof(Settings))] [Required]
        public StatSettings StatSettings;

        private readonly Dictionary<InventoryItemTypes, int>
            _inventoryItems = new Dictionary<InventoryItemTypes, int>();

        private readonly Dictionary<BonusItemTypes, int> _bonusItems = new Dictionary<BonusItemTypes, int>();

        protected bool IsItemExists => _inventoryItems.Count + _bonusItems.Count != 0;
        private bool IsInventoryItem(InventoryItemTypes behaviourType) => _inventoryItems.ContainsKey(behaviourType);
        private bool IsBonusItem(BonusItemTypes behaviourType) => _bonusItems.ContainsKey(behaviourType);

        protected override void Awake()
        {
            if (TargetCamera == null)
                if (Camera.main != null)
                    TargetCamera = Camera.main.transform;

            foreach (var item in ItemSettings.InventoryItems)
            {
                if (!GameSettings.ITEM_ALWAYS_WORKING && !item.IsWorking) continue;
                _inventoryItems.Add(item.ItemType, item.Lifetime);
            }

            foreach (var bonus in ItemSettings.BonusItems)
            {
                if (!GameSettings.ITEM_ALWAYS_WORKING && !bonus.IsWorking) continue;
                _bonusItems.Add(bonus.BonusType, bonus.Lifetime);
            }

            base.Awake();
        }

        protected override void Initialization()
        {
            _services.SaveData = DefaultServices.SaveData;
            _services.Pool = DefaultServices.Pool;
            _services.Random = DefaultServices.Random;
            _services.Localization = DefaultServices.Localization;

            _settings.FlightSettings = FlightSettings;
            _settings.TrapSettings = TrapSettings;
            _settings.WallSettings = WallSettings;
            _settings.ScoreSettings = ScoreSettings;
            _settings.BloodSettings = BloodSettings;
            _settings.DiamondSettings = DiamondSettings;
            _settings.ItemSettings = ItemSettings;
            _settings.StatSettings = StatSettings;

            CreateSystems();

            base.Initialization();
        }

        protected virtual void CreateSystems()
        {
            OpenSystemGroup("ConfigSystems");
            {
                Add(new ConfigInitSystem(_contexts, _services, _settings, SkipStepCount));
                Add(new FinishOnGameOverSystem(_contexts));
                Add(new ScoreRegionSystem(_contexts, _settings));
                Add(new SaveDataSystem(_contexts, _settings));
            }
            CloseSystemGroup();

            if (IsItemExists)
            {
                OpenSystemGroup("ItemSystems");
                {
                    Add(new ItemSystem(_contexts, _settings));
                    if (IsBonusItem(BonusItemTypes.Efficiency))
                        Add(new EfficiencyItemSystem(_contexts, _settings));
                    if (IsBonusItem(BonusItemTypes.MagnitudeField))
                        Add(new MagnitudeDiamondSystem(_contexts));
                }
                CloseSystemGroup();
            }

            CreateFlightSystems();
            CreateEnvironmentSystems();

            if (IsItemExists)
            {
                OpenSystemGroup("ItemSystems");
                {
                    if (IsInventoryItem(InventoryItemTypes.Sonar))
                        Add(new SonarItemSystem(_contexts, _settings, TargetCamera));
                    if (IsInventoryItem(InventoryItemTypes.Cocoon))
                        Add(new CocoonOnCollidingSystem(_contexts, _settings));
                    if (IsInventoryItem(InventoryItemTypes.AdditionalLife))
                        Add(new LifeItemSystem(_contexts, _settings));
                }
                CloseSystemGroup();
            }

            OpenSystemGroup("OtherSystems");
            {
                //Add(new ConfigCleanupSystems(_contexts));
                //Add(new FlightCleanupSystems(_contexts));
                //Add(new EnvironmentCleanupSystems(_contexts));
            }
            CloseSystemGroup();
        }

        protected virtual void CreateFlightSystems()
        {
            OpenSystemGroup("FlightSystems");
            {
                Add(new PoolSystem(_services, GameSettings.POOL_ID_FLIGHT));
                Add(new FlightInitSystem(_contexts, _services, _settings, TargetCamera));
                Add(new EnvironmentPositionSystem(_contexts));
                Add(new AccelerationFlightSystem(_contexts));
                Add(new FollowSystem(_contexts));
                Add(new GuideMovementSystem(_contexts));
                Add(new MovementSystem(_contexts));
                Add(new PinSystem(_contexts));
                Add(new LookAtSystem(_contexts));
                Add(new LimitSpeedFlightSystem(_contexts));
                Add(new DeathStatusSystem(_contexts));
                Add(new DeathSystem(_contexts, _services));
                Add(new ResurrectionAfterDeathSystem(_contexts));
                Add(new ResurrectionSystem(_contexts, _services, _settings, TargetCamera));
                Add(new GameOverSystem(_contexts));

                OpenSystemGroup("InputSystems");
                {
                    Add(new InputInitSystem(_contexts, _settings));
                    Add(new InputMovementSystem(_contexts));
                    Add(new InputSwitchSystem(_contexts));
                    Add(new InputUpdateSpeedSystem(_contexts, _settings));
                    Add(new InputRateSystem(_contexts));
                }
                CloseSystemGroup();

                OpenSystemGroup("BloodSystems");
                {
                    Add(new PoolSystem(_services, GameSettings.POOL_ID_FLIGHT_BLOODS));
                    Add(new BloodSpawnSystem(_contexts, _services, _settings, TargetCamera));
                }
                CloseSystemGroup();

                if (IsInventoryItem(InventoryItemTypes.Luck))
                {
                    OpenSystemGroup("ItemSystems");
                    Add(new LuckSpeedSystem(_contexts, _settings));
                    CloseSystemGroup();
                }
            }
            CloseSystemGroup();
        }

        protected virtual void CreateEnvironmentSystems()
        {
            OpenSystemGroup("EnvironmentSystems");
            {
                Add(new GenerationSystem(_contexts));
                Add(new DestroySystem(_contexts, _services));
                Add(new AccelerationEnvironmentSystem(_contexts));
                Add(new LimitSpeedEnvironmentSystem(_contexts));

                CreateWallSystems();
                CreateTrapSystems();
                CreateDiamondSystems();
                CreateScoreSystems();
            }
            CloseSystemGroup();
        }

        protected virtual void CreateWallSystems()
        {
            OpenSystemGroup("WallSystems");
            {
                Add(new PoolSystem(_services, GameSettings.POOL_ID_ENVIRONMENT_WALLS));
                Add(new WallInitializeSystem(_contexts, _services, _settings, TargetCamera));
                Add(new WallGenerationSystem(_contexts, _services, _settings, TargetCamera));
                Add(new WallRandomSystem(_contexts, _settings));
            }
            CloseSystemGroup();
        }

        protected virtual void CreateTrapSystems()
        {
            OpenSystemGroup("TrapSystems");
            {
                Add(new PoolSystem(_services, GameSettings.POOL_ID_ENVIRONMENT_TRAPS));
                Add(new FOVSystem(_contexts, TargetCamera));
                Add(new FOVEnabledEventSystem(_contexts));
                Add(new RotationSystem(_contexts));
                Add(new GenerationMemorySystem(_contexts, _services, _settings));

                if (IsItemExists)
                {
                    OpenSystemGroup("ItemSystems");
                    {
                        if (IsInventoryItem(InventoryItemTypes.Cocoon))
                            Add(new CocoonGenerationSystem(_contexts, _services, _settings));
                        if (IsInventoryItem(InventoryItemTypes.Luck))
                            Add(new LuckDifficultySystem(_contexts, _services, _settings));
                        if (IsInventoryItem(InventoryItemTypes.Sonar))
                            Add(new SonarRequestSystem(_contexts, _services, _settings));
                    }
                    CloseSystemGroup();
                }

                Add(new GenerationTrapSystem(_contexts, _services, _settings, TargetCamera));
            }
            CloseSystemGroup();
        }

        protected virtual void CreateDiamondSystems()
        {
            OpenSystemGroup("DiamondSystems");
            {
                Add(new PoolSystem(_services, GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS));
                Add(new DiamondGenerationSystem(_contexts, _services, _settings, TargetCamera));
                Add(new DiamondOnCollidingSystem(_contexts, _services, _settings, TargetCamera));
            }
            CloseSystemGroup();
        }

        protected virtual void CreateScoreSystems()
        {
            OpenSystemGroup("ScoreSystems");
            {
                Add(new PoolSystem(_services, GameSettings.POOL_ID_ENVIRONMENT_SCORE));
                Add(new ScoreGenerationSystem(_contexts, _settings, TargetCamera));
                Add(new ScoreSpawnSystem(_contexts, _services, _settings, TargetCamera));
            }
            CloseSystemGroup();
        }

        protected override void TearDown()
        {
            base.TearDown();

            foreach (var context in _contexts.allContexts)
            {
                context.RemoveAllEventHandlers();
                context.DestroyAllEntities();
            }

            Services.Dispose();
            Settings.Dispose();

            GameSettings.PlayCount++;
        }
    }
}