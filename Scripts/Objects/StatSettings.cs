using UnityEngine;

namespace GameEngine
{
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(StatSettings))]
    public sealed class StatSettings : ScriptableObject
    {
        /// Параметр с самым первым рекордом
        public CountParameter FirstScoreParameter;

        /// Параметр с последним рекордом
        public CountParameter LastScoreParameter;

        /// Параметр с самым большим рекордом
        public CountParameter HighScoreParameter;

        /// Кол-во совершенных рекордов (когда побит старый результат)
        public CountParameter RecordCountParameter;

        /// Кол-во совершенных матчей
        public CountParameter PlayCountParameter;

        /// Текущее кол-во алмазов
        public CountParameter DiamondParameter;

        /// Сколько всего алмазов найдено за все игры
        public CountParameter DiamondFoundParameter;

        /// Кол-во смертей персонажей
        public CountParameter DeathCountParameter;

        /// Максимальный пробег персонажами без потери ни одного из них
        public CountParameter LongestScoreRaceParameter;
    }
}