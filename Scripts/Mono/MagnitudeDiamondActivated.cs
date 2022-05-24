using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Обрабатывает столкновения магнитного поля с алмазами.
    /// </summary>
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(MagnitudeDiamondActivated))]
    public sealed class MagnitudeDiamondActivated : MonoBehaviour
    {
        [Required] public ItemSettings ItemSettings;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!collider.CompareTag(GameSettings.TAG_DIAMOND)) return;

            var diamond = collider.GetComponent<MagnitudeDiamond>();
            if (diamond == null) return;

            diamond.Colliding(this, collider.gameObject, ItemSettings);
        }
    }
}