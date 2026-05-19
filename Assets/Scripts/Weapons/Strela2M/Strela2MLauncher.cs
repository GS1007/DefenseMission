using System;
using System.Collections;
using UnityEngine;

public class Strela2MLauncher : MonoBehaviour
{
    public static event Action<Strela2MMissile> MissileLoaded;
    public static event Action Fired;
    public static event Action IllegallyFired;
    public static event Action<LaunchMode> LaunchModeSet;
    public static event Action<bool> MissileLaunched;

    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private Transform _angleSetupPoint;
    [SerializeField] private Transform _scanPoint;

    [SerializeField] private LayerMask _aircraftLayer;

    [SerializeField] private Strela2MMissile _missilePrefab;

    [SerializeField] private float _lauchModeSetupTime = 0f;

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

        Fire();
    }

    private void Fire()
    {
        Strela2MSeeker seeker = LoadedMissile.Seeker;

        if (_launchMode == LaunchMode.Automatic && seeker.HasLock == false)
        {
            return;
        }

        Transform finalTarget = null;

        if (seeker.HasLock == true && seeker.CurrentTarget != null)
        {
            // if (seeker.CurrentTarget.TryGetComponent(out IAircraftTarget aircraftTarget) == true)
            // {
            //     bool hit = CheckAngle();

            //     Debug.Log(hit);

            //     finalTarget = hit ? aircraftTarget.GetSweetSpot() : aircraftTarget.GetDamagePoint();

            //     MissileLaunched?.Invoke(hit);
            // }
            // else
            // {
            //     finalTarget = seeker.CurrentTarget.transform;
            // }

            finalTarget = seeker.CurrentTarget.transform;
        }

        LoadedMissile.Launch(finalTarget);
        LoadedMissile = null;
        State = LauncherState.Off;
        _triggerIsHeld = false;

        Fired?.Invoke();
    }

    private bool CheckAngle()
    {
        return Physics.SphereCast(_angleSetupPoint.position, 10f, _angleSetupPoint.forward, out RaycastHit hit, 4000f, _aircraftLayer);
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
}
