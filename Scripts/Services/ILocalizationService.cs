using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// Паттерн сервиса по работае с локализацией.
    /// </summary>
    public interface ILocalizationService
    {
        /// Возвращает строку по заданному ключу
        string GetTranslation(string key);

        /// Возвращает строку по заданному ключу и заменить подстроки параметров в ней
        string GetTranslation(string key, params object[] parameters);

        /// Возвращает список всех существующих ключей
        List<string> GetTermsList();
    }
}