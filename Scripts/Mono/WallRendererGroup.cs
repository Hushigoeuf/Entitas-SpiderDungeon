using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(WallRendererGroup))]
    public sealed class WallRendererGroup : MonoBehaviour
    {
        [Required] public SpriteRenderer FirstWallRenderer;
        [Required] public SpriteRenderer SecondWallRenderer;
        [Required] public SpriteRenderer ThirdWallRenderer;
    }
}