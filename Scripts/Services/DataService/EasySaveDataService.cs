namespace GameEngine
{
    public sealed class ESDataService : IDataService
    {
        public bool KeyExists(string key)
        {
            return ES3.KeyExists(key);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return ES3.Load<int>(key, defaultValue);
        }

        public void SaveInt(string key, int value)
        {
            ES3.Save<int>(key, value);
        }

        public string LoadString(string key, string defaultValue = null)
        {
            return ES3.Load<string>(key, defaultValue);
        }

        public void SaveString(string key, string value)
        {
            ES3.Save<string>(key, value);
        }

        public bool LoadBool(string key, bool defaultValue = false)
        {
            return ES3.Load<bool>(key, defaultValue);
        }

        public void SaveBool(string key, bool value)
        {
            ES3.Save<bool>(key, value);
        }
    }
}