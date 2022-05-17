using UnityEngine;

namespace GameEngine
{
    public sealed class UnityTimeService : ITimeService
    {
        public float DeltaTime => Time.deltaTime;
        public float FixedDeltaTime => Time.fixedDeltaTime;
        public float RealtimeSinceStartup => Time.realtimeSinceStartup;
    }
}