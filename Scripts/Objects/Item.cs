using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public abstract class Item : ScriptableObject
    {
        [Required] public Sprite MainSprite;

        [ValueDropdown(nameof(EditorGetDescriptionKeys))]
        public string DescriptionKey;

        [Range(0, 10)] public int ViewIndexForContentPopup = 0;

        public Color ColorForContentPopup = Color.white;

        public virtual bool CanDrop() => true;

        public virtual void SaveDropValue(int value)
        {
        }

        public virtual string GetPopupDropString(int value) => null;

#if UNITY_EDITOR
        private List<string> EditorGetDescriptionKeys() => DefaultServices.Localization.GetTermsList();
#endif
    }
}