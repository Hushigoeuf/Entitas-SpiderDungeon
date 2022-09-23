using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(CountParameter))]
    public class CountParameter : Parameter
    {
        [MinValue(nameof(MinValueIfEnabled))] [MaxValue(nameof(MaxValueIfEnabled))] [SerializeField]
        private int _defaultValue;

        public bool MinLimitEnabled = true;

        [EnableIf(nameof(MinLimitEnabled))] [MaxValue(nameof(MaxValueIfEnabled))]
        public int MinValue;

        public bool MaxLimitEnabled;

        [EnableIf(nameof(MaxLimitEnabled))] [MinValue(nameof(MinValueIfEnabled))]
        public int MaxValue;

        public int DefaultValue => Mathf.Clamp(_defaultValue, MinValueIfEnabled, MaxValueIfEnabled);
        public int MinValueIfEnabled => MinLimitEnabled ? MinValue : int.MinValue;
        public int MaxValueIfEnabled => MaxLimitEnabled ? MaxValue : int.MaxValue;
        public bool IsExists => SaveDataProvider.IsExistsInt(Key, DefaultValue);

        public int Value
        {
            get => Mathf.Clamp(SaveDataProvider.GetInt(Key, DefaultValue), MinValueIfEnabled, MaxValueIfEnabled);
            set => SaveDataProvider.SetInt(Key, Mathf.Clamp(value, MinValueIfEnabled, MaxValueIfEnabled));
        }
    }
}