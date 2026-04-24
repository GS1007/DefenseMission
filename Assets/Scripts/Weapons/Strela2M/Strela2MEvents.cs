using System;

public static class Strela2MEvents
{
    public static event Action<int> BatteryHealthUpdated;
    public static event Action BatteryDied;
    public static event Action TargetLocked;
    public static event Action LockReseted;
    public static event Action TrackingStarted;
    public static event Action TrackingStopped;
    public static event Action Fired;

    public static void TriggerBatteryHealthUpdateEvent(int batteryHealth)
    {
        BatteryHealthUpdated?.Invoke(batteryHealth);
    }

    public static void TriggerBatteryDeathEvent()
    {
        BatteryDied?.Invoke();
    }

    public static void TriggerTrackingStartEvent()
    {
        TrackingStarted?.Invoke();
    }

    public static void TriggerTrackingStopEvent()
    {
        TrackingStopped?.Invoke();
    }

    public static void TriggerLockEvent()
    {
        TargetLocked?.Invoke();
    }

    public static void TriggerLockResetEvent()
    {
        LockReseted?.Invoke();
    }

    public static void TriggerFireEvent()
    {
        Fired?.Invoke();
    }
}
