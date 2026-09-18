using UnityEngine;

public interface ISeeker
{
    public Transform CurrentTarget { get; }
    public TargetType CurrentTargetType { get; }
    public float SignalStrength { get; }
    public float LockProgress { get; }
    public bool HasLock { get; }

    public void ProcessSeekerFrame(float deltaTime);
    public void ResetSeeker();
}
