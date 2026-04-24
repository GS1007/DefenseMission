using System.Collections;
using UnityEngine;

public class Strela2MController : MonoBehaviour
{
    [SerializeField] private GameObject _lockLight;

    [SerializeField] private Transform _missileSpawnPoint;

    [SerializeField] private Strela2MBattery _battery;
    [SerializeField] private Strela2MMissile _missilePrefab;
    [SerializeField] private Strela2MSeeker _seeker;

    [SerializeField] private float _automaticModeAngleSetupTime = 0f;
    [SerializeField] private float _manualModeAngleSetupTime = 0f;

    private bool _isAutomaticMode = false;
    private bool _triggerPullingStarted = false;
    private bool _triggerPullingEnded = false;
    private bool _isModeCheckOver = false;

    private Transform _missileHitPoint;

    private WaitForSeconds _fireModeCheckDelay;

    private void Awake()
    {
        _fireModeCheckDelay = new WaitForSeconds(1f);
    }

    private void OnEnable()
    {
        Strela2MEvents.BatteryDied += ResetWeapon;
        Strela2MEvents.TargetLocked += EnableLockLight;
        Strela2MEvents.LockReseted += DisableLockLight;

        Strela2MInput.TriggerPullingStarted += OnTiggerPullingStart;
        Strela2MInput.TriggerPullingEnded += OnTriggerPullingEnd;
    }

    private void Update()
    {
        if (_battery.IsPoweredOn == true && _triggerPullingEnded == true && _isModeCheckOver == true)
        {
            if (_isAutomaticMode == true)
            {
                if (_seeker.IsLocked == true)
                {
                    _missileHitPoint = _seeker.GetHitPoint();

                    StartCoroutine(StartMissileLaunching());

                    _triggerPullingEnded = false;
                }
            }
            else
            {
                if (_seeker.IsLocked == true)
                {
                    _missileHitPoint = _seeker.GetHitPoint();
                    StartCoroutine(StartMissileLaunching());
                }
                else
                {
                    Strela2MMissile missile = Instantiate(_missilePrefab, _missileSpawnPoint.position, _missileSpawnPoint.rotation);
                    missile.MissTarget();
                    ResetWeapon();

                    Strela2MEvents.TriggerFireEvent();
                }

                _triggerPullingEnded = false;
            }
        }
    }

    private void OnDisable()
    {
        Strela2MInput.TriggerPullingEnded -= OnTriggerPullingEnd;
        Strela2MInput.TriggerPullingStarted -= OnTiggerPullingStart;

        Strela2MEvents.BatteryDied -= ResetWeapon;
        Strela2MEvents.TargetLocked -= EnableLockLight;
        Strela2MEvents.LockReseted -= DisableLockLight;
    }

    private void LaunchMissile()
    {
        Strela2MMissile missile = Instantiate(_missilePrefab, _missileSpawnPoint.position, _missileSpawnPoint.rotation);
        missile.LaunchTowardsTarget(_missileHitPoint);
    }

    private void OnTiggerPullingStart()
    {
        if (_battery.IsPoweredOn == true && _triggerPullingStarted == false)
        {
            StartCoroutine(TriggerPullingStartRoutine());

            _triggerPullingStarted = true;
        }
    }
    private void OnTriggerPullingEnd()
    {
        if (_battery.IsPoweredOn == true && _triggerPullingEnded == false)
        {
            _triggerPullingEnded = true;
        }
    }

    private void ResetWeapon()
    {
        _isModeCheckOver = false;
        _isAutomaticMode = false;
        _triggerPullingStarted = false;
        _triggerPullingEnded = false;
        _missileHitPoint = null;
    }

    private void EnableLockLight()
    {
        _lockLight.SetActive(true);
    }

    private void DisableLockLight()
    {
        _lockLight.SetActive(false);
    }

    private IEnumerator TriggerPullingStartRoutine()
    {
        yield return _fireModeCheckDelay;

        if (_triggerPullingEnded == true)
        {
            _isAutomaticMode = true;
        }

        if (_isAutomaticMode == true)
        {
            Debug.Log("Automatic Mode");
        }
        else
        {
            Debug.Log("Manual Mode");
        }

        _isModeCheckOver = true;
    }

    private IEnumerator StartMissileLaunching()
    {
        float angleSetupTime = _isAutomaticMode ? _automaticModeAngleSetupTime : _manualModeAngleSetupTime;

        Debug.Log($"Angle setup time: {angleSetupTime}");

        DisableLockLight();

        yield return new WaitForSeconds(angleSetupTime);

        LaunchMissile();

        Strela2MEvents.TriggerFireEvent();

        ResetWeapon();

        Debug.Log("Fired");
    }
}
