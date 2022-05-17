namespace GameEngine
{
    public partial class Data
    {
        private static Data _sharedInstance;

        public static Data sharedInstance
        {
            get
            {
                if (_sharedInstance == null) _sharedInstance = new Data();

                return _sharedInstance;
            }
        }

        public static void Dispose()
        {
            _sharedInstance = null;
        }
    }

    public partial class Data
    {
        public FlightData FlightData;
        public WallData WallData;
        public TrapData TrapData;
        public ScoreData ScoreData;
        public BloodData BloodData;
        public DiamondData DiamondData;
        public ContentSettingsObject ContentData;
        public StatsData StatsData;
    }
}