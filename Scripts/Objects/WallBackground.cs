using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(WallBackground))]
    public class WallBackground : ScriptableObject
    {
        [Range(0, 1)] public float Chance = 1;

        [GUIColor(1f / 255 * 60, 1f / 255 * 179, 1f / 255 * 113)] [Required]
        public Sprite GreenSprite;

        [GUIColor(1f / 255 * 70, 1f / 255 * 130, 1f / 255 * 180)] [Required]
        public Sprite BlueSprite;

        [GUIColor(1f / 255 * 255, 1f / 255 * 165, 1f / 255 * 0)] [Required]
        public Sprite OrangeSprite;

        [GUIColor(1f / 255 * 199, 1f / 255 * 21, 1f / 255 * 133)] [Required]
        public Sprite PurpleSprite;

        [GUIColor(1f / 255 * 220, 1f / 255 * 20, 1f / 255 * 60)] [Required]
        public Sprite RedSprite;

        [GUIColor(1f / 255 * 255, 1f / 255 * 255, 1f / 255 * 255)] [Required]
        public Sprite WhiteSprite;

        public Sprite GetSprite(int step)
        {
            if (step <= 0) return GreenSprite;
            switch (step)
            {
                case 1:
                    return OrangeSprite;
                case 2:
                    return RedSprite;
                case 3:
                    return PurpleSprite;
                case 4:
                    return BlueSprite;
                default:
                    return WhiteSprite;
            }
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void EditorShowPathInformation()
        {
            SirenixEditorGUI.BeginBox();
            {
                var text = "";
                text += (BlueSprite != null ? AssetDatabase.GetAssetPath(BlueSprite) : "...") + "\n";
                text += (GreenSprite != null ? AssetDatabase.GetAssetPath(GreenSprite) : "...") + "\n";
                text += (OrangeSprite != null ? AssetDatabase.GetAssetPath(OrangeSprite) : "...") + "\n";
                text += (PurpleSprite != null ? AssetDatabase.GetAssetPath(PurpleSprite) : "...") + "\n";
                text += (RedSprite != null ? AssetDatabase.GetAssetPath(RedSprite) : "...") + "\n";
                text += WhiteSprite != null ? AssetDatabase.GetAssetPath(WhiteSprite) : "...";

                GUILayout.Label(text);
            }
            SirenixEditorGUI.EndBox();
        }
#endif
    }
}