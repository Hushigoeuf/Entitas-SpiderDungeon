using EasyMobile;
using UnityEngine;

namespace GameEngine
{
    public sealed class EasyMobileService : MonoBehaviour
    {
        private void Awake()
        {
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }
    }
}