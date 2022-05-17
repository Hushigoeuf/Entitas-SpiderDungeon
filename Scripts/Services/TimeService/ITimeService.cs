namespace GameEngine
{
    public interface ITimeService
    {
        float DeltaTime { get; }
        float FixedDeltaTime { get; }
        float RealtimeSinceStartup { get; }
    }
}