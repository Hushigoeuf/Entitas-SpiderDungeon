namespace GameEngine
{
    public static class GameSettings
    {
        #region CONST_PARAMETERS

        public const float GENERATION_GATE_SIZE = 3.5f;
        public const bool ITEM_ALWAYS_WORKING = false;
        public const bool LIFETIME_LESS_ON_MENU = true;
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

        public static int PlayCount;

        #endregion
    }
}