using System;
using System.Collections;
using UnityEngine;

public class Strela2MLauncher : MonoBehaviour
{
    public static event Action<Strela2MMissile> MissileLoaded;
    public static event Action Fired;

    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private Transform _angleSetupPoint;

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
    }

    private void Start()
    {
        _launchModeSetupDelay = new WaitForSeconds(_lauchModeSetupTime);
        LoadMissile();
    }

    private void Update()
    {
        if (State != LauncherState.Ready || LoadedMissile == null)
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
            State = LauncherState.Ready;
        }
    }

    private void OnTriggerPullingStart()
    {
        if (State != LauncherState.Ready || LoadedMissile == null)
        {
            return;
        }

        StartCoroutine(SetLaunchMode());
    }

    private void OnTroggerPullingEnd()
    {
        if (State != LauncherState.Ready || _triggerIsHeld == true || LoadedMissile == null)
        {
            return;
        }

        Fire();

        _triggerIsHeld = true;
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
            if (seeker.CurrentTarget.TryGetComponent(out IAircraftTarget aircraftTarget) == true)
            {
                finalTarget = CheckAngle() ? aircraftTarget.GetSweetSpot() : aircraftTarget.GetDamagePoint();
            }
            else
            {
                finalTarget = seeker.CurrentTarget.transform;
            }
        }

        LoadedMissile.Launch(finalTarget);
        LoadedMissile = null;
        State = LauncherState.Empty;
        _triggerIsHeld = false;

        Fired?.Invoke();
    }

    private bool CheckAngle()
    {
        return Physics.Raycast(_angleSetupPoint.position, _angleSetupPoint.forward, 4000f, _aircraftLayer);
    }

    private IEnumerator SetLaunchMode()
    {
        yield return _launchModeSetupDelay;

        _launchMode = _triggerIsHeld ? LaunchMode.Automatic : LaunchMode.Manual;
    }
}
