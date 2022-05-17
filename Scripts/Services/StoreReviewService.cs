using EasyMobile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public sealed class StoreReviewService : MonoBehaviour
    {
        private static bool _enabled = true;
        private static float _timeAtLastRequest = -1;

        [BoxGroup("_checker", false)] [Required] [SerializeField]
        private CheckerObject _checker;

        [BoxGroup("_timeout", false)] [MinValue(0)] [SerializeField]
        private float _timeout;

        private void Awake()
        {
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        private void Start()
        {
            if (_timeAtLastRequest == -1)
                _timeAtLastRequest = Time.time;
        }

        public void TryShowRatingPopup()
        {
            if (!_enabled) return;
            if (!_checker.Check()) return;
            var currentTime = Time.time;
            if (currentTime - _timeAtLastRequest < _timeout) return;

            _enabled = false;
            _timeAtLastRequest = currentTime;
            
#if UNITY_EDITOR
            Debug.Log("RequestRating...");
#endif

#if !UNITY_EDITOR
            StoreReview.RequestRating();
#endif
        }
    }
}