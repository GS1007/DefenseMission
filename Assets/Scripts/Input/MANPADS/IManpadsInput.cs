using System;

public interface IManpadsInput
{
    public event Action PowerToggled;
    public event Action TriggerPullingStarted;
    public event Action TriggerPullingEnded;
    public event Action LauncherReseted;
    public event Action TrackingReseted;
}
