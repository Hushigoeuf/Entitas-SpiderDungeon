using System.Collections;
using EasyMobile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameEngine
{
    public sealed class ShareService : EntitasBehaviour
    {
        private const string SHARING_IMAGE_NAME = "SharingScreenshot";

        [BoxGroup("_highScoreParameter", false)] [Required] [SerializeField]
        private CountParameterObject _highScoreParameter;

        [BoxGroup("_upOffset", false)] [MinValue(0)] [SerializeField]
        private int _upOffset;

        // -------------------------------------------------------------------------------------------------------------

        public void StartSharing()
        {
            StartCoroutine(_SaveScreenshot());
        }

        private IEnumerator _SaveScreenshot()
        {
            yield return new WaitForEndOfFrame();
            var path = Sharing.SaveScreenshot(0, 0, GameSettings.ScreenWidth,
                GameSettings.ScreenHeight - GameSettings.ScreenHeight / GameSettings.TARGET_HEIGHT * _upOffset,
                SHARING_IMAGE_NAME);
            Sharing.ShareImage(path,
                DefaultServices.Localization.GetTranslation("Sharing_DefaultMessage", _highScoreParameter.Value),
                DefaultServices.Localization.GetTranslation("Sharing_DefaultSubject"));
        }
    }
}