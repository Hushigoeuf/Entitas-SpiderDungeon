using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Параметр типа "статус".
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(StatusParameter))]
    public sealed class StatusParameter : Parameter
    {
        /// Значение параметра по умолчанию
        [SerializeField] private bool _defaultValue = false;

        public bool DefaultValue => _defaultValue;
        public bool IsExists => SaveDataProvider.IsExistsBool(Key, DefaultValue);

        /// Текущее значение параметра (автоматически отправляет данные в провайдер)
        public bool Value
        {
            get => SaveDataProvider.GetBool(Key, DefaultValue);
            set => SaveDataProvider.SetBool(Key, value);
        }
    }
}