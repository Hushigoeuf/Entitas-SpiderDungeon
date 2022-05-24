using System.Collections.Generic;
#if I2
using I2.Loc;
#endif

namespace GameEngine
{
    /// <summary>
    /// Сервис для работы с локализацией на основе I2 Localization.
    /// Источник: https://assetstore.unity.com/packages/tools/localization/i2-localization-14884
    /// </summary>
    public sealed class I2LocalizationService : ILocalizationService
    {
        public string GetTranslation(string key)
        {
#if I2
            return LocalizationManager.GetTranslation(key, true, 0, true, true);
#endif
            return null;
        }

        public string GetTranslation(string key, params object[] parameters)
        {
#if I2
            return string.Format(GetTranslation(key), parameters);
#endif
            return null;
        }

        public List<string> GetTermsList()
        {
#if I2
            return LocalizationManager.GetTermsList();
#endif
            return null;
        }
    }
}