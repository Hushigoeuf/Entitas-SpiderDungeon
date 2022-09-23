using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public abstract class Parameter : ScriptableObject
    {
        protected static ISaveDataService DefaultDataService = new EasySaveDataService();
        protected static ISaveDataService CurrentDataService => DefaultDataService;

        [ReadOnly] public string KeyMD5;

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