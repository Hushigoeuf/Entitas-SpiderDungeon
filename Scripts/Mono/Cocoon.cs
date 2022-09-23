using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(Cocoon))]
    public class Cocoon : MonoBehaviour
    {
        [Required] public GameObject DefaultModel;

        [Required] public GameObject ModelOnColliding;

        private void OnEnable()
        {
            DefaultModel.SetActive(true);
            ModelOnColliding.SetActive(false);
        }
    }
}