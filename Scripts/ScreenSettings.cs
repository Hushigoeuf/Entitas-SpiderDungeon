using UnityEngine;

namespace GameEngine
{
    public static class ScreenSettings
    {
        #region CONST_PARAMETERS

        public const float TARGET_SCREEN_WIDTH = 1536;
        public const float TARGET_SCREEN_HEIGHT = 2048;
        public const float TARGET_SCREEN_ASPECT = TARGET_SCREEN_HEIGHT / TARGET_SCREEN_WIDTH;
        public const float SAFE_SCREEN_WIDTH = 1080f;
        public const float SAFE_SCREEN_HEIGHT = 1920f;
        public const float PIXELS_PER_UNIT = 100;

        #endregion

        #region DYNAMIC_PARAMETERS

        public static float CurrentScreenWidth => Screen.width;
        public static float CurrentScreenHeight => Screen.height;
        public static float CurrentScreenAspect => CurrentScreenHeight / CurrentScreenWidth;
        public static float CurrentTransformWidth => PixelsToUnits(CurrentScreenWidth);
        public static float CurrentTransformHeight => PixelsToUnits(CurrentScreenAspect * TARGET_SCREEN_WIDTH);

        #endregion

        public static float PixelsToUnits(float value) => value / PIXELS_PER_UNIT;

        public static float PixelsToUnits(int value) => PixelsToUnits((float) value);

        public static float GetSafeScreenWidth() => CurrentScreenWidth;

        public static float GetSafeScreenHeight()
        {
            var screenHeight = CurrentScreenHeight;
            var resultHeight = CurrentScreenWidth * (SAFE_SCREEN_HEIGHT / SAFE_SCREEN_WIDTH);
            if (resultHeight > screenHeight) resultHeight = screenHeight;
            return resultHeight;
        }

        public static float GetTopScreenPoint(float offsetPosition = 0) =>
            offsetPosition + CurrentTransformHeight / 2;

        public static float GetBottomScreenPoint(float offsetPosition = 0) =>
            offsetPosition - CurrentTransformHeight / 2;

        public static float GetLeftScreenPoint(float offsetPosition = 0) =>
            offsetPosition - CurrentTransformWidth / 2;

        public static float GetRightScreenPoint(float offsetPosition = 0) =>
            offsetPosition + CurrentTransformWidth / 2;
    }
}