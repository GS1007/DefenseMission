using System;
using System.Collections;
using UnityEngine;

public class Strela2MLauncher : MonoBehaviour
{
    public static event Action<Strela2MMissile> MissileLoaded;
    public static event Action Fired;
    public static event Action IllegallyFired;
    public static event Action<LaunchMode> LaunchModeSet;

    [SerializeField] private GameObject _angleSetupStick;

    [SerializeField] private Transform _strela2M;
    [SerializeField] private Transform _missileSpawnPoint;
    [SerializeField] private Transform _angleSetupPoint;

    [SerializeField] private LayerMask _aircraftLayer;

    [SerializeField] private Strela2MMissile _missilePrefab;
    [SerializeField] private Strela2MInput _input;

    [SerializeField] private float _lauchModeSetupTime = 0f;
    [SerializeField] private float _angleSetupFOV = 0f;
    [SerializeField] private float _automaticLaunchTime = 0f;
    [SerializeField] private float _zRotationLimit = 0f;
    [SerializeField] private float _trackingLockResetTime = 1f;

    private bool _triggerIsHeld = false;
    private bool _trackingIsAllowed = true;

    private LaunchMode _launchMode = LaunchMode.Automatic;

    private WaitForSeconds _launchModeSetupDelay;
    private WaitForSeconds _automaticLaunchDelay;
    private WaitForSeconds _trackingLockResetDelay;

    private Strela2MSeeker _seeker;

    private IEnumerator _launchModeSetRoutine;

    public LauncherState State { get; set; } = LauncherState.Off;
    public Strela2MMissile LoadedMissile { get; private set; }
    public Strela2MSeeker CurrentSeeker { get { return _seeker; } }

    private void OnEnable()
    {
        Strela2MBattery.PowerUpStarted += OnBatteryPowerupStart;
        Strela2MBattery.PoweredOn += OnBatteryPowerOn;
        Strela2MInput.TriggerPullingStarted += OnTriggerPullingStart;
        Strela2MInput.TriggerPullingEnded += OnTroggerPullingEnd;
        Strela2MBattery.BatteryDied += OnBatteryDeath;
        Strela2MInput.LauncherReseted += OnLaucherReset;
        Strela2MInput.TrackingReseted += OnTrackingReset;
        Strela2MInput.AngleSetupStickToggled += OnAngleSetupStickToggle;
    }

    private void Start()
    {
        _launchModeSetupDelay = new WaitForSeconds(_lauchModeSetupTime);
        _automaticLaunchDelay = new WaitForSeconds(_automaticLaunchTime);
        _trackingLockResetDelay = new WaitForSeconds(_trackingLockResetTime);
    }

    private void Update()
    {
        if (State != LauncherState.Ready)
        {
            return;
        }

        if(_trackingIsAllowed == true)
        {
            _seeker.DoUpdate();
        }

        if (_triggerIsHeld == true)
        {
            LaunchModeSet?.Invoke(_launchMode);

            if (_launchMode == LaunchMode.Manual)
            {
                Fire();
            }
            else
            {
                StartCoroutine(LaunchAutomaticMode());
            }

            if (_launchModeSetRoutine != null)
            {
                StopCoroutine(_launchModeSetRoutine);
                _launchModeSetRoutine = null;
            }

            _launchMode = LaunchMode.Automatic;
            _triggerIsHeld = false;
        }
    }

    private void OnDisable()
    {
        Strela2MInput.TriggerPullingEnded -= OnTroggerPullingEnd;
        Strela2MInput.TriggerPullingStarted -= OnTriggerPullingStart;
        Strela2MBattery.PoweredOn -= OnBatteryPowerOn;
        Strela2MBattery.PowerUpStarted -= OnBatteryPowerupStart;
        Strela2MBattery.BatteryDied -= OnBatteryDeath;
        Strela2MInput.LauncherReseted -= OnLaucherReset;
        Strela2MInput.TrackingReseted -= OnTrackingReset;
        Strela2MInput.AngleSetupStickToggled -= OnAngleSetupStickToggle;
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
        if (State != LauncherState.Ready)
        {
            IllegallyFired?.Invoke();

            return;
        }

        if(_launchModeSetRoutine == null)
        {
            _launchModeSetRoutine = SetManualLaunchMode();
            StartCoroutine(_launchModeSetRoutine);
        }
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
        bool isCriticalHit = IsWithinTheAngle() && Mathf.Abs(Mathf.DeltaAngle(0, _strela2M.eulerAngles.z)) <= _zRotationLimit;

        if (isCriticalHit == false && _seeker.CurrentTargetType == TargetType.Aircraft)
        {
            _seeker.ResetSeeker();
        }

        LoadedMissile.Launch(isCriticalHit);
        LoadedMissile = null;
        State = LauncherState.Off;

        Fired?.Invoke();
    }

    private IEnumerator SetManualLaunchMode()
    {
        yield return _launchModeSetupDelay;

        _launchMode = LaunchMode.Manual;
    }

    private IEnumerator LaunchAutomaticMode()
    {
        yield return _automaticLaunchDelay;

        if (_seeker.HasLock == true)
        {
            Fire();
        }
    }

    private IEnumerator ResetTrackingLock()
    {
        yield return _trackingLockResetDelay;

        if (_trackingIsAllowed == false)
        {
            _trackingIsAllowed = true;
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

    private void OnLaucherReset()
    {
        _triggerIsHeld = false;
    }

    private void OnTrackingReset()
    {
        if(_seeker == null)
        {
            return;
        }

        if(_trackingIsAllowed == true)
        {
            StartCoroutine(ResetTrackingLock());

            _seeker.ResetSeeker();
            _trackingIsAllowed = false;
        }
    }

    private void OnAngleSetupStickToggle()
    {
        _angleSetupStick.SetActive(!_angleSetupStick.activeSelf);
    }
}
