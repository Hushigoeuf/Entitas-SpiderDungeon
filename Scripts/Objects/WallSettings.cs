using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(WallSettings))]
    public class WallSettings : ScriptableObject
    {
        public GameObject WallPrefab;

        public int PixelsPerWall;

        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowPaging = false, ShowIndexLabels = false)]
        [SerializeField]
        public WallBackground[] Backgrounds = new WallBackground[0];

        private List<Sprite> _result;

        public List<Sprite> GetRandomWallSprites(int level, int minCount)
        {
            if (level < 0) return null;
            if (minCount < 0) return null;

            if (_result == null)
                _result = new List<Sprite>();
            else _result.Clear();
            if (minCount == 0) return _result;

            var backgrounds = Backgrounds;
            DefaultServices.Random.Shuffle(ref backgrounds);

            for (var i = 0; i < backgrounds.Length; i++)
            {
                if (DefaultServices.Random.IsChance(Backgrounds[i].Chance))
                {
                    _result.Add(backgrounds[i].GetSprite(level));
                    if (_result.Count >= minCount) break;
                }

                if (i == backgrounds.Length - 1) i = 0;
            }

            return _result;
        }
    }
}