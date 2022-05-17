using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public sealed class GameEntitasManager : EntitasManager<GameEntitasManager>
    {
#if UNITY_EDITOR
        private static string _editorFirstBlockLabel = "Logic Rules";
#endif

        [BoxGroup("$_editorFirstBlockLabel")]
        [BoxGroup("$_editorFirstBlockLabel/_fightIncluded", false)]
        [SerializeField]
        private bool _fightIncluded;

        [BoxGroup("_cameraTransform", false)] [Required] [SerializeField]
        private Transform _cameraTransform;

        [BoxGroup("_soundManager", false)] [DisableIf("$_fightIncluded")] [SerializeField]
        private SoundManager _soundManager;

        // -------------------------------------------------------------------------------------------------------------

        [BoxGroup("Data Objects")] [BoxGroup("Data Objects/_flightData", false)] [Required] [SerializeField]
        private FlightData _flightData;

        [BoxGroup("Data Objects/_wallEnvironmentData", false)] [Required] [SerializeField]
        private WallData _wallData;

        [BoxGroup("Data Objects/_trapData", false)] [Required] [SerializeField]
        private TrapData _trapData;

        [BoxGroup("Data Objects/_scoreData", false)] [Required] [SerializeField]
        private ScoreData _scoreData;

        [BoxGroup("Data Objects/_bloodData", false)] [Required] [SerializeField]
        private BloodData _bloodData;

        [BoxGroup("Data Objects/_diamondData", false)] [Required] [SerializeField]
        private DiamondData _diamondData;

        [BoxGroup("Data Objects/_contentData", false)] [Required] [SerializeField]
        private ContentSettingsObject _contentData;

        [BoxGroup("Data Objects/_statsData", false)] [Required] [SerializeField]
        private StatsData _statsData;

        [BoxGroup("Data Objects/_fightData", false)] [Required] [SerializeField]
        private FightData _fightData;

        // -------------------------------------------------------------------------------------------------------------

        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = false)] [SerializeField]
        private MonoBehaviour[] _delayControllers = new MonoBehaviour[0];

        // -------------------------------------------------------------------------------------------------------------

        private readonly Dictionary<BonusBehaviourTypes, int> _bonusList = new Dictionary<BonusBehaviourTypes, int>();
        private readonly Dictionary<ItemBehaviourTypes, int> _itemList = new Dictionary<ItemBehaviourTypes, int>();
        private int _delay;

        protected override void Awake()
        {
            if (!_fightIncluded)
                foreach (var bonus in _contentData.BonusList)
                {
                    if (!GameSettings.CONTENT_ALWAYS_INCLUDED && !bonus.IsInclude()) continue;
                    _bonusList.Add(bonus.BehaviourType, bonus.Lifetime);
                }

            if (!_fightIncluded)
                foreach (var item in _contentData.ItemList)
                {
                    if (!GameSettings.CONTENT_ALWAYS_INCLUDED && !item.IsInclude()) continue;
                    _itemList.Add(item.BehaviourType, item.Lifetime);
                }

            if (_delayControllers.Length != 0)
                for (var i = 0; i < _delayControllers.Length; i++)
                {
                    var delay = ((IDelayController) _delayControllers[i]).GetDelay();
                    if (delay > _delay) _delay = delay;
                }

            _delayControllers = null;

            base.Awake();
        }

        private bool IsBonus(BonusBehaviourTypes behaviourType)
        {
            return !_fightIncluded && _bonusList.ContainsKey(behaviourType);
        }

        private bool IsItem(ItemBehaviourTypes behaviourType)
        {
            return !_fightIncluded && _itemList.ContainsKey(behaviourType);
        }

        protected override void Initialize()
        {
            // Инициализируем необходимые сервисы.
            {
                Services.CameraService = new UnityCameraService(_cameraTransform);
                Services.PoolService = DefaultServices.Pool;
                Services.RandomService = DefaultServices.Random;
                Services.SceneService = DefaultServices.Scene;
                Services.TimeService = DefaultServices.Time;
            }

            // Определяем ссылки на объекты игровых данных.
            {
                Data.FlightData = _flightData;
                Data.TrapData = _trapData;
                Data.WallData = _wallData;
                Data.ScoreData = _scoreData;
                Data.BloodData = _bloodData;
                Data.DiamondData = _diamondData;
                Data.ContentData = _contentData;
                Data.StatsData = _statsData;
            }

            // Создаем необходимые системы для работы игры.
            _CreateSystems();

            base.Initialize();
        }

        private void _CreateSystems()
        {
            StartSystemGroup("ConfigSystems");
            {
                Add(new InitializeConfigSystem(Contexts, Services, Data, _delay));
                Add(new GameOverConfigSystem(Contexts));
                if (!_fightIncluded)
                {
                    Add(new RegionConfigSystem(Contexts, Data, _soundManager));
                    Add(new SaveStatsConfigSystem(Contexts, Data));
                }

                //Add(new CashStorageConfigSystem(Contexts, Services, Data));
                //Add(new RegionConfigSystem(Contexts, Data));
            }
            EndSystemGroup();

            StartSystemGroup("ContentSystems");
            {
                if (_bonusList.Count + _itemList.Count != 0)
                    Add(new ContentObjectConfigSystem(Contexts, Services, Data));
                if (IsBonus(BonusBehaviourTypes.Efficiency))
                    Add(new EfficiencyConfigSystem(Contexts, Services, Data));
                if (IsBonus(BonusBehaviourTypes.Magnitude))
                    Add(new MagnitudeDiamondEnvironmentSystem(Contexts, Services));
            }
            EndSystemGroup();

            _CreateFlightSystems();
            _CreateEnvironmentSystems();

            StartSystemGroup("ContentSystems");
            {
                if (IsItem(ItemBehaviourTypes.Sonar)) Add(new SonarCleanupConfigSystem(Contexts, Services, Data));

                if (IsItem(ItemBehaviourTypes.Resurrection))
                    Add(new CoconLifeRequestConfigSystem(Contexts, Data));
                if (IsItem(ItemBehaviourTypes.Life))
                    Add(new LifeItemConfigSystem(Contexts, Data));
            }
            EndSystemGroup();

            StartSystemGroup("MainSystems");
            {
                Add(new ConfigCleanupSystems(Contexts));
                Add(new FlightCleanupSystems(Contexts));
                Add(new EnvironmentCleanupSystems(Contexts));
            }
            EndSystemGroup();
        }

        private void _CreateFlightSystems()
        {
            StartSystemGroup("FlightSystems");
            {
                Add(new PoolSystem(Services, GameSettings.POOL_ID_FLIGHT));
                Add(new InitializeFlightSystem(Contexts, Services, Data));
                Add(new EnvironmentPositionFlightSystem(Contexts));
                Add(new AccelerationFlightSystem(Contexts, Services));
                Add(new FollowFlightSystem(Contexts, Services));
                Add(new GuideOffsetFlightSystem(Contexts, Services));
                Add(new MovementFlightSystem(Contexts, Services));
                Add(new PinFlightSystem(Contexts));
                Add(new RotationFlightSystem(Contexts, Services));
                Add(new UpdateSpeedFlightSystem(Contexts));
                Add(new DeadStatusFlightSystem(Contexts));
                Add(new DeadFlightSystem(Contexts, Services, Data));
                Add(new ResurrectionAfterDeathFlightSystem(Contexts));
                Add(new ResurrectionFlightSystem(Contexts, Services, Data));
                Add(new GameOverFlightSystem(Contexts));

                StartSystemGroup("InputSystems");
                {
                    Add(new InitializeInputFlightSystem(Contexts, Data));
                    Add(new InputOffsetFlightSystem(Contexts, Services));
                    Add(new UpdateGuideOffsetFlightSystem(Contexts));
                    Add(new UpdateInputOffsetFlightSystem(Contexts, Services, Data.FlightData));
                    Add(new UpdateRateFlightSystem(Contexts));
                }
                EndSystemGroup();

                StartSystemGroup("BloodSystems");
                {
                    Add(new PoolSystem(Services, GameSettings.POOL_ID_FLIGHT_BLOODS));
                    Add(new BloodFlightSystem(Contexts, Services, Data));
                }
                EndSystemGroup();

                StartSystemGroup("ContentSystems");
                {
                    if (IsItem(ItemBehaviourTypes.Luck))
                        Add(new LuckFlightSystem(Contexts, Data));
                }
                EndSystemGroup();
            }
            EndSystemGroup();
        }

        private void _CreateEnvironmentSystems()
        {
            StartSystemGroup("EnvironmentSystems");
            {
                Add(new GenerationEnvironmentSystem(Contexts));
                Add(new DestroyEnvironmentSystem(Contexts, Services));
                Add(new AccelerationEnvironmentSystem(Contexts, Services));
                Add(new UpdateSpeedEnvironmentSystem(Contexts));

                __CreateWallSystems();

                if (!_fightIncluded)
                {
                    __CreateTrapSystems();
                    __CreateDiamondSystems();
                    __CreateScoreSystems();
                }
            }
            EndSystemGroup();
        }

        private void __CreateWallSystems()
        {
            StartSystemGroup("WallSystems");
            {
                Add(new PoolSystem(Services, GameSettings.POOL_ID_ENVIRONMENT_WALLS));
                Add(new InitializeWallEnvironmentSystem(Contexts, Services, Data.WallData));
                Add(new GenerationWallEnvironmentSystem(Contexts, Services, Data.WallData));
                Add(new WallEnvironmentSystem(Contexts, Data));
            }
            EndSystemGroup();
        }

        private void __CreateTrapSystems()
        {
            StartSystemGroup("TrapSystems");
            {
                Add(new PoolSystem(Services, GameSettings.POOL_ID_ENVIRONMENT_TRAPS));
                Add(new FOVEnvironmentSystem(Contexts, Services));
                Add(new FOVEnabledEventSystem(Contexts));
                Add(new RotationEnvironmentSystem(Contexts, Services));
                //Add(new GenerationMemoryVer1EnvironmentSystem(Contexts, Services, Data));
                //Add(new GenerationMemoryVer2EnvironmentSystem(Contexts, Services, Data));
                Add(new GenerationMemoryVer3EnvironmentSystem(Contexts, Services, Data));

                StartSystemGroup("ContentSystems");
                {
                    if (IsItem(ItemBehaviourTypes.Resurrection))
                        Add(new CoconGenerationEnvironmentSystem(Contexts, Services, Data));
                    if (IsItem(ItemBehaviourTypes.Luck))
                        Add(new LuckEnvironmentSystem(Contexts, Services, Data));
                    if (IsItem(ItemBehaviourTypes.Sonar))
                        Add(new SonarRequestEnvironmentSystem(Contexts, Services, Data));
                }
                EndSystemGroup();

                Add(new GenerationTrapEnvironmentSystem(Contexts, Services, Data));
            }
            EndSystemGroup();
        }

        private void __CreateDiamondSystems()
        {
            StartSystemGroup("DiamondSystems");
            {
                Add(new PoolSystem(Services, GameSettings.POOL_ID_ENVIRONMENT_DIAMONDS));
                Add(new GenerationDiamondEnvironmentSystem(Contexts, Services, Data));
                Add(new DiamondEnvironmentSystem(Contexts, Services, Data));
            }
            EndSystemGroup();
        }

        private void __CreateScoreSystems()
        {
            StartSystemGroup("ScoreSystems");
            {
                Add(new PoolSystem(Services, GameSettings.POOL_ID_ENVIRONMENT_SCORE));
                Add(new InitializeScoreEnvironmentSystem(Contexts, Services, Data));
                Add(new GenerationScoreEnvironmentSystem(Contexts, Services, Data));
            }
            EndSystemGroup();
        }

        protected override void TearDown()
        {
            base.TearDown();

            foreach (var context in Contexts.allContexts)
            {
                context.RemoveAllEventHandlers();
                context.DestroyAllEntities();
            }

            Services.Dispose();
            Data.Dispose();

            GameSettings.PlayCount++;
            GameSettings.IsDebugMode = false;

            if (!_fightIncluded)
                AnalyticsService.OnGameStop(_statsData.LastScoreParameter.Value, _statsData.DiamondParameter.Value);
            else
                AnalyticsService.OnFightStop(_fightData.FightStatusParameter.Value);
        }
    }
}