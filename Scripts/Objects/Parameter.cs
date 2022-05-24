using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Базовый класс, который задает объект как параметр.
    /// Такие параметры можно создавать и использовать в разных местах,
    /// чтобы управлять одним и тем же источником.
    /// </summary>
    public abstract class Parameter : ScriptableObject
    {
        protected static ISaveDataService DefaultDataService = new EasySaveDataService();
        protected static ISaveDataService CurrentDataService => DefaultDataService;

        /// Ключ в формате MD5 по которому данные будут загружаться и сохраняться
        [ReadOnly] public string KeyMD5;

        /// Ключ для внешнего использования
        public string Key
        {
            get
            {
#if UNITY_EDITOR
                return name;
#endif
                return KeyMD5;
            }
        }

#if UNITY_EDITOR
        [OnInspectorInit]
        private void EditorOnInitKey()
        {
            KeyMD5 = FormatUtility.ToMd5Hash(GetInstanceID() + "");
        }
#endif
    }
}