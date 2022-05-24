namespace GameEngine
{
    public partial class Settings
    {
        private static Settings _sharedInstance;

        public static Settings sharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                    _sharedInstance = new Settings();
                return _sharedInstance;
            }
        }

        public static void Dispose()
        {
            _sharedInstance = null;
        }
    }

    public partial class Settings
    {
        public FlightSettings FlightSettings;
        public TrapSettings TrapSettings;
        public WallSettings WallSettings;
        public ScoreSettings ScoreSettings;
        public BloodSettings BloodSettings;
        public DiamondSettings DiamondSettings;
        public ItemSettings ItemSettings;
        public StatSettings StatSettings;
    }
}