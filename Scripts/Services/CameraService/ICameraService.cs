using UnityEngine;

namespace GameEngine
{
    public interface ICameraService
    {
        Camera Camera { get; }
        Transform CameraTransform { get; }
        Transform Container { get; }

        void TakeShake(string preset);
    }
}