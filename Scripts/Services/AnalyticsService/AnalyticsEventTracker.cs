using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public sealed class AnalyticsEventTracker : CustomMonoBehaviour
    {
        [BoxGroup("_customEventName", false)] [SerializeField]
        private string _customEventName;

        [BoxGroup("_sendOnEnable", false)] [SerializeField]
        private bool _sendOnEnable;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_sendOnEnable)
                SendEvent();
        }

        public void SendEvent()
        {
            if (!string.IsNullOrEmpty(_customEventName))
                AnalyticsService.SendEvent(_customEventName);
        }
    }
}