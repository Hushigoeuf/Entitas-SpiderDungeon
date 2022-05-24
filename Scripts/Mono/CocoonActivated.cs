using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Обрабатывает столкновения персонажа с коконами.
    /// </summary>
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(CocoonActivated))]
    public sealed class CocoonActivated : GameEntitasBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag(GameSettings.TAG_COCOON)) return;
            if (!IsMainConfig) return;
            if (!IsGameOver) return;

            var cocoon = collider.GetComponent<Cocoon>();
            if (cocoon == null) return;

            cocoon.DefaultModel.SetActive(false);
            cocoon.ModelOnColliding.SetActive(true);

            var e = _contexts.config.CreateEntity();
            e.isCocoonLifeRequest = true;
        }
    }
}