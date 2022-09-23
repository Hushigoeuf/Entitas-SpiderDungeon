using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(StatusParameter))]
    public class StatusParameter : Parameter
    {
        [SerializeField] private bool _defaultValue = false;

        public bool DefaultValue => _defaultValue;
        public bool IsExists => SaveDataProvider.IsExistsBool(Key, DefaultValue);

        public bool Value
        {
            get => SaveDataProvider.GetBool(Key, DefaultValue);
            set => SaveDataProvider.SetBool(Key, value);
        }
    }
}