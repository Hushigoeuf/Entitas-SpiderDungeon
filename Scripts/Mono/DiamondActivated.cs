using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Обрабатывает столкновения персонажа с алмазами.
    /// </summary>
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(DiamondActivated))]
    public class DiamondActivated : GameEntitasBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag(GameSettings.TAG_DIAMOND)) return;
            if (!IsMainConfig) return;
            if (!IsGameOver) return;

            var diamond = collider.GetComponent<Diamond>();
            if (diamond == null) return;

            var e = _contexts.environment.CreateEntity();
            {
                e.isDiamond = true;
                e.AddTransform(diamond.CustomTarget ? diamond.CustomTarget : diamond.transform);
                e.AddIndex(diamond.CostTypeIndex);
            }
        }
    }
}