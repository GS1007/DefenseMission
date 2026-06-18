using System;
using System.Collections;
using UnityEngine;

public class Strela2MLauncher : MonoBehaviour
{
    public static event Action<Strela2MMissile> MissileLoaded;
    public static event Action Fired;
    public static event Action IllegallyFired;
    public static event Action<LaunchMode> LaunchModeSet;

    [SerializeField] private Transform _strela2M;
    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private Transform _angleSetupPoint;

    [SerializeField] private LayerMask _aircraftLayer;

    [SerializeField] private Strela2MMissile _missilePrefab;

    [SerializeField] private float _lauchModeSetupTime = 0f;
    [SerializeField] private float _angleSetupFOV = 0f;
    [SerializeField] private float _automaticLaunchTime = 0f;
    [SerializeField] private float _zRotationLimit = 0f;

    private bool _modeCheckingStarted = false;
    private bool _triggerIsHeld = false;

    private LaunchMode _launchMode = LaunchMode.Automatic;

    private WaitForSeconds _launchModeSetupDelay;
    private WaitForSeconds _automaticLaunchDelay;

    public LauncherState State { get; set; } = LauncherState.Off;
    public Strela2MMissile LoadedMissile { get; private set; }

    private Strela2MSeeker _seeker;

    public Strela2MSeeker CurrentSeeker { get { return _seeker; } }

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
        _automaticLaunchDelay = new WaitForSeconds(_automaticLaunchTime);
    }

    private void Update()
    {
        if (State != LauncherState.Ready)
        {
            return;
        }

        _seeker.DoUpdate();

        if (_triggerIsHeld == true)
        {
            if (_launchMode == LaunchMode.Manual)
            {
                Fire();
            }
            else
            {
                _triggerIsHeld = false;
                StartCoroutine(LaunchAutomaticMode());
            }
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
        _seeker = LoadedMissile.Seeker;
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

        if (_modeCheckingStarted == false)
        {
            StartCoroutine(SetLaunchMode());
            _modeCheckingStarted = true;
        }
    }

    private void OnTroggerPullingEnd()
    {
        if (State != LauncherState.Ready || _triggerIsHeld == true)
        {
            return;
        }

        _triggerIsHeld = true;

        Debug.Log(_launchMode);
    }

    private void Fire()
    {
        bool isCriticalHit = IsWithinTheAngle() && Mathf.Abs(Mathf.DeltaAngle(0, _strela2M.eulerAngles.z)) <= _zRotationLimit;

        if (isCriticalHit == false && _seeker.CurrentTargetType == TargetType.Aircraft)
        {
            _seeker.ResetSeeker();
        }

        LoadedMissile.Launch(isCriticalHit);
        LoadedMissile = null;
        State = LauncherState.Off;
        _triggerIsHeld = false;
        _modeCheckingStarted = false;

        Fired?.Invoke();
    }

    private IEnumerator SetLaunchMode()
    {
        yield return _launchModeSetupDelay;

        _launchMode = _triggerIsHeld ? LaunchMode.Automatic : LaunchMode.Manual;

        LaunchModeSet?.Invoke(_launchMode);
    }

    private IEnumerator LaunchAutomaticMode()
    {
        yield return _automaticLaunchDelay;

        if (_seeker.HasLock == true)
        {
            Fire();
        }
    }

    private bool IsWithinTheAngle()
    {
        Transform target = LoadedMissile.Seeker.CurrentTarget;

        if (target == null)
        {
            return false;
        }

        Vector3 direction = (target.position - _angleSetupPoint.position).normalized;

        return Vector3.Angle(_angleSetupPoint.forward, direction) <= _angleSetupFOV;
    }

    private void OnBatteryDeath()
    {
        State = LauncherState.Off;
        _triggerIsHeld = false;
    }
}
