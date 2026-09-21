using UnityEngine;

public interface ISeeker
{
    public Transform CurrentTarget { get; }
    public TargetType CurrentTargetType { get; }
    public bool HasLock { get; }
    public float SignalStrength { get; }
    public float LockProgress { get; }
    public float TargetLockTime { get; }
    public void SetTrackingActive(bool isActive);
    public void ResetSeeker();
}
