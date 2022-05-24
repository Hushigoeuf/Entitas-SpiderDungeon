#define DOTWEEN
using Entitas;
#if DOTWEEN
using DG.Tweening;
#endif

namespace GameEngine
{
    [Environment]
    public class DOTweenAnimationComponent : IComponent
    {
#if DOTWEEN
        public DOTweenAnimation Value;
#endif
    }
}