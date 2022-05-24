namespace GameEngine
{
    /// <summary>
    /// Паттерн сервиса для работы с реестром сохранений.
    /// </summary>
    public interface ISaveDataService
    {
        /// Существует ли параметр в реестре сохранений
        bool KeyExists(string key);

        /// Загружает значение параметра из реестра сохранений
        int LoadInt(string key, int defaultValue = 0);

        /// Сохраняет параметр в реестре сохранений
        void SaveInt(string key, int value);

        string LoadString(string key, string defaultValue = null);
        void SaveString(string key, string value);
        bool LoadBool(string key, bool defaultValue = false);
        void SaveBool(string key, bool value);
    }
}