using System;
using System.Collections;
using UnityEngine;

public class Strela2MLauncher : MonoBehaviour
{
    public static event Action<Strela2MMissile> MissileLoaded;
    public static event Action Fired;
    public static event Action IllegallyFired;
    public static event Action<LaunchMode> LaunchModeSet;

    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private Transform _angleSetupPoint;

    [SerializeField] private LayerMask _aircraftLayer;

    [SerializeField] private Strela2MMissile _missilePrefab;

    [SerializeField] private float _lauchModeSetupTime = 0f;
    [SerializeField] private float _angleSetupFOV = 0f;

    private bool _triggerIsHeld = false;

    private LaunchMode _launchMode = LaunchMode.Automatic;

    private WaitForSeconds _launchModeSetupDelay;

    public LauncherState State { get; set; } = LauncherState.Off;
    public Strela2MMissile LoadedMissile { get; private set; }

    private void OnEnable()
    {
        Strela2MBattery.PowerUpStarted += OnBatteryPowerupStart;
        Strela2MBattery.PoweredOn += OnBatteryPowerOn;
        Strela2MInput.TriggerPullingStarted += OnTriggerPullingStart;
        Strela2MInput.TriggerPullingEnded += OnTroggerPullingEnd;
        Strela2MBattery.BatteryDied += OnBatteryDeath;
    }

    private void Start()
    {
        _launchModeSetupDelay = new WaitForSeconds(_lauchModeSetupTime);
    }

    private void Update()
    {
        if (State != LauncherState.Ready)
        {
            return;
        }

        LoadedMissile.Seeker.DoUpdate();

        if (_triggerIsHeld == true && (_launchMode == LaunchMode.Manual || (_launchMode == LaunchMode.Automatic && LoadedMissile.Seeker.HasLock == true)))
        {
            Fire();
        }
    }

    private void OnDisable()
    {
        Strela2MInput.TriggerPullingEnded -= OnTroggerPullingEnd;
        Strela2MInput.TriggerPullingStarted -= OnTriggerPullingStart;
        Strela2MBattery.PoweredOn -= OnBatteryPowerOn;
        Strela2MBattery.PowerUpStarted -= OnBatteryPowerupStart;
        Strela2MBattery.BatteryDied -= OnBatteryDeath;
    }

    private void LoadMissile()
    {
        LoadedMissile = Instantiate(_missilePrefab, _missileSpawnPoint);
        MissileLoaded?.Invoke(LoadedMissile);
    }

    private void OnBatteryPowerupStart()
    {
        State = LauncherState.SpinningUp;
    }

    private void OnBatteryPowerOn()
    {
        if (State != LauncherState.DeadBattery)
        {

            if (LoadedMissile == null)
            {
                LoadMissile();
            }

            State = LauncherState.Ready;
        }
    }

    private void OnTriggerPullingStart()
    {
        IllegallyFired?.Invoke();

        if (State != LauncherState.Ready)
        {
            return;
        }

        StartCoroutine(SetLaunchMode());
    }

    private void OnTroggerPullingEnd()
    {
        if (State != LauncherState.Ready || _triggerIsHeld == true)
        {
            return;
        }

        _triggerIsHeld = true;
    }

    private void Fire()
    {
        Strela2MSeeker seeker = LoadedMissile.Seeker;
        bool isCriticalHit = CalculateAngleSetup(LoadedMissile.Seeker.CurrentTarget) <= _angleSetupFOV;

        LoadedMissile.Launch(isCriticalHit);
        LoadedMissile = null;
        State = LauncherState.Off;
        _triggerIsHeld = false;

        Fired?.Invoke();

        Debug.Log(CalculateAngleSetup(LoadedMissile.Seeker.CurrentTarget));
    }

    private IEnumerator SetLaunchMode()
    {
        yield return _launchModeSetupDelay;

        _launchMode = _triggerIsHeld ? LaunchMode.Automatic : LaunchMode.Manual;

        LaunchModeSet?.Invoke(_launchMode);
    }

    private void OnBatteryDeath()
    {
        State = LauncherState.Off;
        _triggerIsHeld = false;
    }

    private float CalculateAngleSetup(Transform target)
    {
        Vector3 direction = (target.position - _angleSetupPoint.position).normalized;

        float angle = Vector3.Angle(_angleSetupPoint.forward, direction);

        return angle;
    }
}
