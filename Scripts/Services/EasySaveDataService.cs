namespace GameEngine
{
    /// <summary>
    /// Сервис для работы с реестром сохранений на основе EasySave 3.
    /// Источник: https://assetstore.unity.com/packages/tools/utilities/easy-save-the-complete-save-data-serialization-system-768
    /// </summary>
    public class EasySaveDataService : ISaveDataService
    {
        public bool KeyExists(string key) => ES3.KeyExists(key);

        public int LoadInt(string key, int defaultValue = 0) => ES3.Load<int>(key, defaultValue);

        public void SaveInt(string key, int value)
        {
            ES3.Save<int>(key, value);
        }

        public string LoadString(string key, string defaultValue = null) => ES3.Load<string>(key, defaultValue);

        public void SaveString(string key, string value)
        {
            ES3.Save<string>(key, value);
        }

        public bool LoadBool(string key, bool defaultValue = false) => ES3.Load<bool>(key, defaultValue);

        public void SaveBool(string key, bool value)
        {
            ES3.Save<bool>(key, value);
        }
    }
}