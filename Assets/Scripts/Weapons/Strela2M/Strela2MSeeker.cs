using System.Collections;
using UnityEngine;

public class Strela2MSeeker : MonoBehaviour
{
    [SerializeField] private Transform _trackOrigin;
    [SerializeField] private Transform _seekingRubberPoint;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Strela2MBattery _battery;

    [SerializeField] private float _lockRange;
    [SerializeField] private float _trackRadius;
    [SerializeField] private float _angleSetupRadius;
    [SerializeField] private float _lockTimeRequired;

    private Transform _currentTarget;

    private WaitForSeconds _lockResetDelay;

    private Coroutine _resetLockRoutine;

    private bool _trackingStarted = false;
    private bool _isLocked = false;

    private float _lockTimer = 0f;

    public bool IsLocked { get { return _isLocked; } }

    private void Awake()
    {
        _lockResetDelay = new WaitForSeconds(3f);
    }

    private void OnEnable()
    {
        Strela2MEvents.BatteryDied += ResetSeeker;
        Strela2MEvents.Fired += ResetSeeker;
    }

    private void Update()
    {
        if (_battery.IsPoweredOn == true)
        {
            Track();
        }
    }

    private void OnDisable()
    {
        Strela2MEvents.Fired -= ResetSeeker;
        Strela2MEvents.BatteryDied -= ResetSeeker;
    }

    private void Track()
    {
        RaycastHit hit;

        if (Physics.SphereCast(_trackOrigin.position, _trackRadius, _trackOrigin.forward, out hit, _lockRange, _targetLayer) == true)
        {
            if (_currentTarget == null)
            {
                _currentTarget = hit.transform;
            }

            if (_trackingStarted == false)
            {
                if (_resetLockRoutine != null)
                {
                    StopCoroutine(_resetLockRoutine);

                    _resetLockRoutine = null;
                }

                _trackingStarted = true;
            }

            if (_isLocked == false)
            {
                _lockTimer += Time.deltaTime;

                if (_lockTimer >= _lockTimeRequired)
                {
                    _lockTimer = 0f;
                    _isLocked = true;

                    Strela2MEvents.TriggerLockEvent();
                }
            }
        }
        else
        {
            if (_trackingStarted == true)
            {
                if (_lockTimer > 0f)
                {
                    _lockTimer = 0f;
                }

                if (_isLocked == true && _resetLockRoutine == null)
                {
                    _resetLockRoutine = StartCoroutine(ResetLock());
                }

                _trackingStarted = false;
            }
        }
    }

    public Transform GetHitPoint()
    {
        IAircraftTarget target = _currentTarget.GetComponent<IAircraftTarget>();

        return Physics.BoxCast(_seekingRubberPoint.position, new Vector3(7f, 1000f, 1000f), _seekingRubberPoint.forward, Quaternion.identity, _lockRange, _targetLayer) ? target.GetSweetSpot() : target.GetDamagePoint();

        //return Physics.SphereCast(_seekingRubberPoint.position, _angleSetupRadius, _seekingRubberPoint.forward, out RaycastHit hit, _lockRange, _targetLayer) ? target.GetSweetSpot() : target.GetDamagePoint();
    }

    private void ResetSeeker()
    {
        _trackingStarted = false;
        _isLocked = false;
        _lockTimer = 0f;
        _currentTarget = null;
    }

    private IEnumerator ResetLock()
    {
        yield return _lockResetDelay;

        _currentTarget = null;
        _isLocked = false;

        Strela2MEvents.TriggerLockResetEvent();
    }
}
