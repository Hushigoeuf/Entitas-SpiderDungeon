using UnityEngine;

namespace GameEngine
{
    public static class ScreenSettings
    {
        #region CONST_PARAMETERS

        /// Целевая ширина экрана (на основе нее будет адаптация под другие разрешения)
        public const float TARGET_SCREEN_WIDTH = 1536;

        /// Целевая высота экрана (на основе нее будет адаптация под другие разрешения)
        public const float TARGET_SCREEN_HEIGHT = 2048;

        public const float TARGET_SCREEN_ASPECT = TARGET_SCREEN_HEIGHT / TARGET_SCREEN_WIDTH;

        /// Целевая ширина игровой зоны (если экран больше, то лишнее место займет UI)
        public const float SAFE_SCREEN_WIDTH = 1080f;

        /// Целевая высота игровой зоны (если экран больше, то лишнее место займет UI)
        public const float SAFE_SCREEN_HEIGHT = 1920f;

        /// Целевое кол-во пикселей на один юнит
        public const float PIXELS_PER_UNIT = 100;

        #endregion

        #region DYNAMIC_PARAMETERS

        /// Реальная ширина экрана на данный момент
        public static float CurrentScreenWidth => Screen.width;

        /// Реальная высота экрана на данный момент
        public static float CurrentScreenHeight => Screen.height;

        public static float CurrentScreenAspect => CurrentScreenHeight / CurrentScreenWidth;
        public static float CurrentTransformWidth => PixelsToUnits(CurrentScreenWidth);
        public static float CurrentTransformHeight => PixelsToUnits(CurrentScreenAspect * TARGET_SCREEN_WIDTH);

        #endregion

        /// <summary>
        /// Конвентирует пиксели в юниты на основе параметра PIXELS_PER_UNIT.
        /// </summary>
        public static float PixelsToUnits(float value) => value / PIXELS_PER_UNIT;

        public static float PixelsToUnits(int value) => PixelsToUnits((float) value);

        /// <summary>
        /// Возвращает ширину игровой зоны.
        /// </summary>
        public static float GetSafeScreenWidth() => CurrentScreenWidth;

        /// <summary>
        /// Вычисляет высоту игровой зоны из текущей высоты экрана.
        /// Это используется для устройств с большой высотой (например, IOS) и лишнее пространство заменяет UI.
        /// </summary>
        public static float GetSafeScreenHeight()
        {
            var screenHeight = CurrentScreenHeight;
            var resultHeight = CurrentScreenWidth * (SAFE_SCREEN_HEIGHT / SAFE_SCREEN_WIDTH);
            if (resultHeight > screenHeight) resultHeight = screenHeight;
            return resultHeight;
        }

        /// <summary>
        /// Возвращает самую верхнюю позицию на экране
        /// </summary>
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