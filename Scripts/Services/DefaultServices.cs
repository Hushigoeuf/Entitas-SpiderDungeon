namespace GameEngine
{
    public static class DefaultServices
    {
        public static readonly IDataService Data = new ESDataService();
        public static readonly ILocalizationService Localization = new I2LocalizationService();
        public static readonly IPoolService Pool = new PathologicalPoolService();
        public static readonly IRandomService Random = new UnityRandomService();
        public static readonly ISceneService Scene = new UnitySceneService();
        public static readonly ITimeService Time = new UnityTimeService();
    }
}