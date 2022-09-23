using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(MagnitudeDiamondActivated))]
    public class MagnitudeDiamondActivated : MonoBehaviour
    {
        [Required] public ItemSettings ItemSettings;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var diamond = collider.GetComponent<MagnitudeDiamond>();
            if (diamond == null) return;

            diamond.Colliding(this, collider.gameObject, ItemSettings);
        }
    }
}