using System.Collections.Generic;
using I2.Loc;

namespace GameEngine
{
    public sealed class I2LocalizationService : ILocalizationService
    {
        public string GetTranslation(string key)
        {
            return LocalizationManager.GetTranslation(key, true, 0, true, true);
        }

        public string GetTranslation(string key, params object[] parameters)
        {
            return string.Format(GetTranslation(key), parameters);
        }

        public List<string> GetTermsList()
        {
            return LocalizationManager.GetTermsList();
        }
    }
}