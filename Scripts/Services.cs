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
        public ITimeService TimeService;
        public IPoolService PoolService;
        public IRandomService RandomService;
        public ICameraService CameraService;
        public ISceneService SceneService;
        public IDataService DataService;
    }
}