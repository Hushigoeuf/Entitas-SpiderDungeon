namespace GameEngine
{
    public interface IDataService
    {
        bool KeyExists(string key);
        int LoadInt(string key, int defaultValue = 0);
        void SaveInt(string key, int value);
        string LoadString(string key, string defaultValue = null);
        void SaveString(string key, string value);
        bool LoadBool(string key, bool defaultValue = false);
        void SaveBool(string key, bool value);
    }
}