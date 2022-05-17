using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public static class GameSettings
    {
        public const float TARGET_WIDTH = 1536;
        public const float TARGET_HEIGHT = 2048;
        public const float PIXELS_PER_UNIT = 100;

        public static float TargetMultiplier => TARGET_HEIGHT / TARGET_WIDTH;
        public static float ScreenWidth => Screen.width;
        public static float ScreenHeight => Screen.height;
        public static float SafeScreenWidth => ScreenWidth;

        public static float SafeScreenHeight
        {
            get
            {
                var screenHeight = ScreenHeight;
                var resultHeight = ScreenWidth * (1920f / 1080f);
                if (resultHeight > screenHeight) resultHeight = screenHeight;
                return resultHeight;
            }
        }

        public static float TransformMultiplier => ScreenHeight / ScreenWidth;
        public static float TransformWidth => ScreenWidth.ToUnits();
        public static float TransformHeight => (TransformMultiplier * TARGET_WIDTH).ToUnits();

        public static float GetTopTransformPoint(float offsetPosition = 0)
        {
            return offsetPosition + TransformHeight / 2;
        }

        public static float GetDownTransformPoint(float offsetPosition = 0)
        {
            return offsetPosition - TransformHeight / 2;
        }

        public static float GetLeftTransformPoint(float offsetPosition = 0)
        {
            return offsetPosition - TransformWidth / 2;
        }

        public static float GetRightTransformPoint(float offsetPosition = 0)
        {
            return offsetPosition + TransformWidth / 2;
        }

        public static float ToUnits(this float pixels)
        {
            return pixels / PIXELS_PER_UNIT;
        }

        public static float ToUnits(this int pixels)
        {
            return ((float) pixels).ToUnits();
        }

        public static float GetResolutionMultiplier(float multiplier = 1f)
        {
            if (multiplier < 0) multiplier = 0;
            var rate = (float) Screen.height / (float) Screen.width -
                       (float) TARGET_HEIGHT / (float) TARGET_WIDTH;
            if (rate < 0) rate = 0;
            return 1 + rate * multiplier;
        }

        // -------------------------------------------------------------------------------------------------------------

        public const float LEVEL_PLACE_SIZE = 3.5f;
        public const bool CONTENT_ALWAYS_INCLUDED = false;

        public const bool DEBUG_AD_ENABLED = true;

        //public const int MIN_DIAMOND_RECEIVE_FOR_GRUBBED = 50;
        //public const float DIAMOND_DROP_SIZE = 0.5f;
        public const bool ITEM_TIME_WORK_ALWAYS = true;
        public const bool ITEM_EQUIP_ALWAYS = true;

        // -------------------------------------------------------------------------------------------------------------

        public const string POOL_ID_ENVIRONMENT_SCORE = "Score";
        public const string POOL_ID_ENVIRONMENT_TRAPS = "Traps";
        public const string POOL_ID_ENVIRONMENT_WALLS = "Walls";
        public const string POOL_ID_ENVIRONMENT_DIAMONDS = "Diamonds";
        public const string POOL_ID_FLIGHT = "Flight";
        public const string POOL_ID_FLIGHT_BLOODS = "Bloods";

        // -------------------------------------------------------------------------------------------------------------

        private static int _playCount = 0;

        public static int PlayCount
        {
            get => _playCount;
            set
            {
                if (value <= _playCount) return;
                _playCount = value;
            }
        }

        public static bool IsDebugMode = false;

        // -------------------------------------------------------------------------------------------------------------

        public static ValueDropdownList<int> ObstacleTypeList = new ValueDropdownList<int>()
        {
            {"Default", 0},
            {"SideLeft", 1},
            {"SideRight", 2},
            {"Fire", 3},
            {"Lightning", 4},
        };
    }
}