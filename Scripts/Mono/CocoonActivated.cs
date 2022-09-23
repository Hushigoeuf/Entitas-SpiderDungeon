using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(CocoonActivated))]
    public class CocoonActivated : GameEntitasBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
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