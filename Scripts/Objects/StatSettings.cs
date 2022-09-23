using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(StatSettings))]
    public class StatSettings : ScriptableObject
    {
        public CountParameter FirstScoreParameter;

        public CountParameter LastScoreParameter;

        public CountParameter HighScoreParameter;

        public CountParameter RecordCountParameter;

        public CountParameter PlayCountParameter;

        public CountParameter DiamondParameter;

        public CountParameter DiamondFoundParameter;

        public CountParameter DeathCountParameter;

        public CountParameter LongestScoreRaceParameter;
    }
}