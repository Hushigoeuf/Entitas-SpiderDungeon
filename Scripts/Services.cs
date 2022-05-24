namespace GameEngine
{
    public partial class Services
    {
        private static Services _sharedInstance;

        public static Services sharedInstance
        {
            get
            {
                if (_sharedInstance == null) _sharedInstance = new Services();

                return _sharedInstance;
            }
        }

        public static void Dispose()
        {
            _sharedInstance = null;
        }
    }

    public partial class Services
    {
        public ISaveDataService SaveData = DefaultServices.SaveData;
        public IPoolService Pool = DefaultServices.Pool;
        public IRandomService Random = DefaultServices.Random;
        public ILocalizationService Localization = DefaultServices.Localization;
    }
}