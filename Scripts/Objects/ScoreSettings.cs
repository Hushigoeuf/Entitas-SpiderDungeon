using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(ScoreSettings))]
    public class ScoreSettings : ScriptableObject
    {
        public GameObject Prefab;
        public int Rate;
    }
}