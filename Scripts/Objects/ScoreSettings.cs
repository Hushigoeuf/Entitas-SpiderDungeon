using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Настройки игрового счета.
    /// </summary>
    [CreateAssetMenu(menuName = nameof(GameEngine) + "/" + nameof(ScoreSettings))]
    public sealed class ScoreSettings : ScriptableObject
    {
        /// Префаб препятствия с котором сталкиваются персонажи для подсчета
        public GameObject Prefab;

        /// Кол-во очков, которые даются за каждое столкновение
        public int Rate;
    }
}