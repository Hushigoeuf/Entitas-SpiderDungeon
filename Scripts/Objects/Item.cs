using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Базовый класс для предметов.
    /// </summary>
    public abstract class Item : ScriptableObject
    {
        /// Базовый спрайт предмета
        [Required] public Sprite MainSprite;

        /// Описание предмета
        [ValueDropdown(nameof(EditorGetDescriptionKeys))]
        public string DescriptionKey;

        /// Индекс представления на странице магазина, который будет отображаться
        [Range(0, 10)] public int ViewIndexForContentPopup = 0;

        /// Цвет предмета, который будет передаваться для отображения его в магазине
        public Color ColorForContentPopup = Color.white;

        /// <summary>
        /// Проверяет возможность выпадения в магазине.
        /// </summary>
        public virtual bool CanDrop() => true;

        /// <summary>
        /// Сохраняет результат после выпадения в магазине.
        /// </summary>
        public virtual void SaveDropValue(int value)
        {
        }

        /// <summary>
        /// Возвращает строку результата для магазина в рамках этого предмета.
        /// </summary>
        public virtual string GetPopupDropString(int value) => null;

#if UNITY_EDITOR
        /// <summary>
        /// Возвращает список ключей для локализации.
        /// </summary>
        private List<string> EditorGetDescriptionKeys() => DefaultServices.Localization.GetTermsList();
#endif
    }
}