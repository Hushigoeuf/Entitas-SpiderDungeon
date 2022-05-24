using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(Diamond))]
    public sealed class Diamond : MonoBehaviour
    {
        public Transform CustomTarget;
        [Range(0, 3)] public int CostTypeIndex;
    }
}