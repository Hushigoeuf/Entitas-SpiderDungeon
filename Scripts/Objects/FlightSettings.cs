using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки отряда (персонажей).
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(FlightSettings))]
    public sealed class FlightSettings : ScriptableObject
    {
        private const string EDITOR_GROUP_GENERAL = "General";
        private const string EDITOR_GROUP_BINDINGS = "Bindings";
        private const string EDITOR_GROUP_INPUT = "Input";

        #region GENERAL_PARAMETERS

        /// Скорость движения отряда
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartSpeed;

        /// Скорость увеличения скорости движения
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float AccelerationSpeed;

        /// Максимальная скорость движения
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float LimitSpeed;

        /// Дополнительный коэффициент, множитель на размер экрана
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float ResolutionSpeedMultiplier;

        /// Максимальное кол-во персонажей в отряде
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(1)]
        public int MaxSizeInFlight;

        /// Стартовое кол-во жизней в запасе
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public int StartLifeInStorage;

        /// Максимальное кол-во жизней в запасе
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(nameof(StartLifeInStorage))]
        public int MaxLifeInStorage;

        /// Стартовая скорость преследования одного персонажа за другим
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartFollowSpeed;

        /// Закрепить значение скорости за стартовым
        [BoxGroup(EDITOR_GROUP_GENERAL)] [SerializeField]
        public bool FollowSpeedPinned;

        /// Дистанция между персонажами
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float FollowDistance;

        /// Стартовая скорость поворота персонажа в сторону другого
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartRotationSpeed;

        /// Закрепить значение скорости за стартовым
        [BoxGroup(EDITOR_GROUP_GENERAL)] public bool RotationSpeedPinned;

        /// Стартовая скорость движения отряда из стороны в сторону
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float StartOffsetSpeed;

        /// Закрепить значение скорости за стартовым
        [BoxGroup(EDITOR_GROUP_GENERAL)] [SerializeField]
        public bool OffsetSpeedPinned;

        /// Максимальный сдвиг в сторону после которого проиходит смена стороны
        [BoxGroup(EDITOR_GROUP_GENERAL)] [MinValue(0)]
        public float OffsetLimit;

        /// Разрешить ли отряду отталкиваться от стен
        [BoxGroup(EDITOR_GROUP_GENERAL)] public bool PushOnSideEnabled;

        /// На какое время отряд отталкивается в противоположную сторону от стены
        [BoxGroup(EDITOR_GROUP_GENERAL)] [EnableIf(nameof(PushOnSideEnabled))] [MinValue(0)]
        public float PushOnSideTimePause;

        #endregion

        #region BINDINGS_PARAMETERS

        /// Префаб первичного персонажа
        [BoxGroup(EDITOR_GROUP_BINDINGS)] public GameObject GuidePrefab;

        /// Префаб дочернего персонажа
        [BoxGroup(EDITOR_GROUP_BINDINGS)] public GameObject CharacterPrefab;

        #endregion

        #region INPUT_PARAMETERS

        /// Скорость движения отряда в сторону с помощью управления
        [BoxGroup(EDITOR_GROUP_INPUT)] [MinValue(0)]
        public float InputSpeed;

        /// Максимальный сдвиг в сторону с помощью управления
        [BoxGroup(EDITOR_GROUP_INPUT)] [MinValue(0)]
        public float InputLimit;

        /// Скорость движения отряда в сторону без участия игрока
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
                var result = StartSpeed * GetResolutionMultiplier(ResolutionSpeedMultiplier);
                if (result > LimitSpeed) result = LimitSpeed;
                return result;
            }
        }

        /// <summary>
        /// Суть в том, чтобы повышать скорость движения если высота экрана большая для баланса.
        /// P.S. В итоге идея себя не оправдала.
        /// </summary>
        private static float GetResolutionMultiplier(float multiplier = 1f)
        {
            multiplier = Mathf.Clamp(multiplier, 0, float.MaxValue);
            var rate = ScreenSettings.CurrentScreenHeight / ScreenSettings.CurrentScreenWidth -
                       ScreenSettings.TARGET_SCREEN_HEIGHT / ScreenSettings.TARGET_SCREEN_WIDTH;
            rate = Mathf.Clamp(rate, 0, float.MaxValue);
            return 1 + rate * multiplier;
        }
    }
}