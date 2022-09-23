namespace GameEngine
{
    public class DefaultServices
    {
        public static readonly ISaveDataService SaveData = new EasySaveDataService();
        public static readonly IPoolService Pool = new PathologicalPoolService();
        public static readonly IRandomService Random = new UnityRandomService();
        public static readonly ILocalizationService Localization = new I2LocalizationService();
    }
}