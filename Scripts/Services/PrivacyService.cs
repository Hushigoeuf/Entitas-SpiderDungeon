using Doozy.Engine;
using EasyMobile;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_IOS
using Balaso;

#endif

namespace GameEngine
{
    public sealed class PrivacyService : MonoBehaviour
    {
#if UNITY_IOS
        private static bool _registerAppForIOSNetwork;
#endif

        [BoxGroup("_acceptButtonContainer", false)] [SerializeField]
        private GameObject _acceptButtonContainer;

        [BoxGroup("_confirmParagraph", false)] [SerializeField]
        private GameObject _confirmParagraph;

        [BoxGroup("_cancelButtonContainer", false)] [SerializeField]
        private GameObject _cancelButtonContainer;

#if UNITY_IOS
        private void Awake()
        {
            if (!_registerAppForIOSNetwork)
            {
                _registerAppForIOSNetwork = true;
                AppTrackingTransparency.RegisterAppForAdNetworkAttribution();
                AppTrackingTransparency.UpdateConversionValue(3);
            }
        }
#endif

        private void Start()
        {
            if (gameObject.scene.buildIndex != 4) return;

            var isGranted = IsGranted();
            if (isGranted) Destroy(_acceptButtonContainer);
            if (isGranted) Destroy(_confirmParagraph);
            if (!isGranted) Destroy(_cancelButtonContainer);

            if (isGranted) _cancelButtonContainer.TrySetActive(true);
            if (!isGranted) _acceptButtonContainer.TrySetActive(true);
        }

        public void _LoadNextSceneFromPrimary()
        {
#if UNITY_IOS
            if (IsGranted())
                if (AppTrackingTransparency.TrackingAuthorizationStatus ==
                    AppTrackingTransparency.AuthorizationStatus.NOT_DETERMINED)
                {
                    AppTrackingTransparency.OnAuthorizationRequestDone += _OnAuthorizationRequestDone1;
                    AppTrackingTransparency.RequestTrackingAuthorization();
                    return;
                }
#endif

            SceneManager.LoadSceneAsync(IsGranted() ? 3 : 4);
        }

#if UNITY_IOS
        private void _OnAuthorizationRequestDone1(AppTrackingTransparency.AuthorizationStatus status)
        {
            SceneManager.LoadSceneAsync(IsGranted() ? 3 : 4);
        }
#endif

        public void _OnAcceptPrivacy()
        {
            if (!IsGranted())
                Privacy.GrantGlobalDataPrivacyConsent();

#if !UNITY_IOS
            GameEventMessage.SendEvent("BackToMenu");
            return;
#endif

#if UNITY_IOS
            if (AppTrackingTransparency.TrackingAuthorizationStatus !=
                AppTrackingTransparency.AuthorizationStatus.AUTHORIZED)
            {
                AppTrackingTransparency.OnAuthorizationRequestDone += _OnAuthorizationRequestDone2;
                AppTrackingTransparency.RequestTrackingAuthorization();
                return;
            }
#endif

            GameEventMessage.SendEvent("BackToMenu");
        }

#if UNITY_IOS
        private void _OnAuthorizationRequestDone2(AppTrackingTransparency.AuthorizationStatus status)
        {
            GameEventMessage.SendEvent("BackToMenu");
        }
#endif

        public static bool IsGranted()
        {
            return Privacy.GlobalDataPrivacyConsent == ConsentStatus.Granted;
        }
    }
}