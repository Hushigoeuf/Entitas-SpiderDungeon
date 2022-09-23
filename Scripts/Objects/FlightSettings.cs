using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(FlightSettings))]
    public class FlightSettings : ScriptableObject
    {
        private const string EDITOR_GROUP_GENERAL = "General";
        private const string EDITOR_GROUP_BINDINGS = "Bindings";
        private const string EDITOR_GROUP_INPUT = "Input";

        #region GENERAL_PARAMETERS

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float AccelerationSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float LimitSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float ResolutionSpeedMultiplier;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(1)]
        public int MaxSizeInFlight;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public int StartLifeInStorage;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(nameof(StartLifeInStorage))]
        public int MaxLifeInStorage;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartFollowSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [SerializeField]
        public bool FollowSpeedPinned;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float FollowDistance;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartRotationSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] public bool RotationSpeedPinned;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartOffsetSpeed;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [SerializeField]
        public bool OffsetSpeedPinned;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float OffsetLimit;

        [BoxGroup(EDITOR_GROUP_GENERAL)] public bool PushOnSideEnabled;

        [BoxGroup(EDITOR_GROUP_GENERAL)] [EnableIf(nameof(PushOnSideEnabled))] [MinValue(0)]
        public float PushOnSideTimePause;

        #endregion

        #region BINDINGS_PARAMETERS

        [BoxGroup(EDITOR_GROUP_BINDINGS)] public GameObject GuidePrefab;

        [BoxGroup(EDITOR_GROUP_BINDINGS)] public GameObject CharacterPrefab;

        #endregion

        #region INPUT_PARAMETERS

        [BoxGroup(EDITOR_GROUP_INPUT)] [MinValue(0)]
        public float InputSpeed;

        [BoxGroup(EDITOR_GROUP_INPUT)] [MinValue(0)]
        public float InputLimit;

        [BoxGroup(EDITOR_GROUP_INPUT)] [MinValue(0)]
        public float InputStartOffset;

        #endregion

        public float FollowSpeed => !FollowSpeedPinned ? StartFollowSpeed : StartFollowSpeed * Speed;
        public float RotationSpeed => !RotationSpeedPinned ? StartRotationSpeed : StartRotationSpeed * Speed;
        public float OffsetSpeed => !OffsetSpeedPinned ? StartOffsetSpeed : StartOffsetSpeed * Speed;

        public float Speed
        {
            get
            {
                float result = StartSpeed * GetResolutionMultiplier(ResolutionSpeedMultiplier);
                if (result > LimitSpeed) result = LimitSpeed;
                return result;
            }
        }

        private static float GetResolutionMultiplier(float multiplier = 1f)
        {
            multiplier = Mathf.Clamp(multiplier, 0, float.MaxValue);
            float rate = ScreenSettings.CurrentScreenHeight / ScreenSettings.CurrentScreenWidth -
                         ScreenSettings.TARGET_SCREEN_HEIGHT / ScreenSettings.TARGET_SCREEN_WIDTH;
            rate = Mathf.Clamp(rate, 0, float.MaxValue);
            return 1 + rate * multiplier;
        }
    }
}