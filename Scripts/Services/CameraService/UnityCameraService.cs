using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

namespace GameEngine
{
    public sealed class UnityCameraService : ICameraService
    {
        private Camera _camera;
        private Transform _cameraTransform;
        private Transform _container;
        private ProCamera2DShake _shakeComponent;

        public Camera Camera => _camera;
        public Transform CameraTransform => _cameraTransform;
        public Transform Container => _container;

        public void TakeShake(string preset)
        {
            _shakeComponent.Shake(preset);
        }

        public UnityCameraService(Transform container)
        {
#if !GE_DEBUG_DISABLED
            if (container == null) throw new CustomArgumentException();
#endif
            Initialize(container.GetComponentInChildren<Camera>(), container);
        }

        public UnityCameraService(Camera camera)
        {
#if !GE_DEBUG_DISABLED
            if (camera == null) throw new CustomArgumentException();
#endif
            Initialize(camera, camera.transform);
        }

        public UnityCameraService()
        {
            Initialize(Camera.main, Camera.main.transform);
        }

        private void Initialize(Camera current, Transform container)
        {
#if !GE_DEBUG_DISABLED
            if (current == null) throw new CustomArgumentException();
            if (container == null) throw new CustomArgumentException();
#endif
            _camera = current;
            _cameraTransform = current.transform;
            _container = container;

            _shakeComponent = _camera.GetComponent<ProCamera2DShake>();
        }
    }
}