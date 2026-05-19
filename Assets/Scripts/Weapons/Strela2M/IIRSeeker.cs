using UnityEngine;

public interface IIRSeeker
{
    public Transform CurrentTarget { get; }
    public bool HasLock { get; }
    public void DoUpdate();
    public void ScanForTargets();
    public void ResetSeeker();
    public float GetLockProgress();
}
