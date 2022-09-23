using System.Collections.Generic;

namespace GameEngine
{
    public interface ILocalizationService
    {
        string GetTranslation(string key);
        string GetTranslation(string key, params object[] parameters);

        List<string> GetTermsList();
    }
}