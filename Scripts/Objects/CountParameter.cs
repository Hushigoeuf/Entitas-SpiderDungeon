using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Параметр типа "счетчик".
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(CountParameter))]
    public sealed class CountParameter : Parameter
    {
        /// Значение параметра по умолчанию
        [MinValue(nameof(MinValueIfEnabled))] [MaxValue(nameof(MaxValueIfEnabled))] [SerializeField]
        private int _defaultValue;

        /// Включить ли минимальное значение
        public bool MinLimitEnabled = true;

        /// Минимальное значение параметра
        [EnableIf(nameof(MinLimitEnabled))] [MaxValue(nameof(MaxValueIfEnabled))]
        public int MinValue;

        /// Включить ли максимальное значение
        public bool MaxLimitEnabled;

        /// Максимальное значение параметра
        [EnableIf(nameof(MaxLimitEnabled))] [MinValue(nameof(MinValueIfEnabled))]
        public int MaxValue;

        public int DefaultValue => Mathf.Clamp(_defaultValue, MinValueIfEnabled, MaxValueIfEnabled);
        public int MinValueIfEnabled => MinLimitEnabled ? MinValue : int.MinValue;
        public int MaxValueIfEnabled => MaxLimitEnabled ? MaxValue : int.MaxValue;
        public bool IsExists => SaveDataProvider.IsExistsInt(Key, DefaultValue);

        /// Текущее значение параметра (автоматически отправляет данные в провайдер)
        public int Value
        {
            get => Mathf.Clamp(SaveDataProvider.GetInt(Key, DefaultValue), MinValueIfEnabled, MaxValueIfEnabled);
            set => SaveDataProvider.SetInt(Key, Mathf.Clamp(value, MinValueIfEnabled, MaxValueIfEnabled));
        }
    }
}