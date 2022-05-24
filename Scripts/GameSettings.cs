namespace GameEngine
{
    public static class GameSettings
    {
        #region CONST_PARAMETERS

        /// Ширина одного прохода если разделить уровень по горизонтали на 4 части
        public const float GENERATION_GATE_SIZE = 3.5f;

        /// Все предметы всегда работают вне зависимости от их состояния
        public const bool ITEM_ALWAYS_WORKING = false;

        /// Должны ли предметы с временем жизни тратить это время в меню игры
        public const bool LIFETIME_LESS_ON_MENU = true;

        /// Предметы всегда экипированы вне зависимости от их состояния
        public const bool ITEM_ALWAYS_EQUIP = true;

        #endregion

        #region TAG_LIST

        public const string TAG_SCORE = "Score";
        public const string TAG_DIAMOND = "Diamond";
        public const string TAG_COCOON = "Cocoon";

        #endregion

        #region POOL_ID_LIST

        public const string POOL_ID_FLIGHT = "Flight";
        public const string POOL_ID_FLIGHT_BLOODS = "Bloods";
        public const string POOL_ID_ENVIRONMENT_SCORE = "Score";
        public const string POOL_ID_ENVIRONMENT_TRAPS = "Traps";
        public const string POOL_ID_ENVIRONMENT_WALLS = "Walls";
        public const string POOL_ID_ENVIRONMENT_DIAMONDS = "Diamonds";

        #endregion

        #region DYNAMIC_PARAMETERS

        /// Кол-во игр за текущую сессию
        public static int PlayCount;

        #endregion
    }
}