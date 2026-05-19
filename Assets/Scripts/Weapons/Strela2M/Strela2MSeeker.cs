using UnityEngine;

public class Strela2MSeeker : MonoBehaviour, IIRSeeker
{
    [Header("Detection Settings")]
    [SerializeField] private float _lockRange = 4000f;
    [SerializeField] private float _seekerFOV = 3.5f;
    [SerializeField] private LayerMask _aircraftLayer;
    [SerializeField] private LayerMask _flareLayer;
    [SerializeField] private LayerMask _occlusionLayers;

    [Header("Design Spec Limits")]
    [SerializeField] private float _lockDuration = 1.2f;
    [SerializeField] private float _maxSlewRate = 60f;
    [SerializeField] private float _seekerTrackRate = 11f;

    [Header("Thermal & Countermeasures")]
    [SerializeField] private float _flareHeatMultiplier = 3.0f;
    [SerializeField] private float _gimbalLimit = 40f;
    [SerializeField] private float _flareTrackDecayRate = 1.5f;
    [SerializeField] private float _signalLockThreshold = 0.15f;

    private float _currentLockTime;
    private Vector3 _lastForward;
    private Vector3 _seekerWorldForward;
    private bool _isUncaged;
    private Transform _sunTransform;

    public Transform CurrentTarget { get; private set; }
    public float SignalStrength { get; private set; }
    public TargetType CurrentTargetType { get; private set; }
    public bool HasLock { get; private set; }

    private void Start()
    {
        var sunObj = GameObject.FindGameObjectWithTag("Sun");
        if (sunObj != null) _sunTransform = sunObj.transform;

        _seekerWorldForward = transform.forward;
        _lastForward = transform.forward;
    }

    public void DoUpdate()
    {
        float frameTurnAngle = Vector3.Angle(_lastForward, transform.forward);
        float turnRatePerSecond = frameTurnAngle / Time.deltaTime;
        _lastForward = transform.forward;

        ScanForTargets();

        if (SignalStrength > _signalLockThreshold && CurrentTarget != null)
        {
            _isUncaged = true;
        }

        if (_isUncaged && CurrentTarget != null)
        {
            Vector3 dirToTarget = (CurrentTarget.position - transform.position).normalized;

            _seekerWorldForward = Vector3.RotateTowards(
                _seekerWorldForward,
                dirToTarget,
                _seekerTrackRate * Mathf.Deg2Rad * Time.deltaTime,
                0f);

            float gimbalAngle = Vector3.Angle(transform.forward, _seekerWorldForward);
            if (gimbalAngle > _gimbalLimit)
            {
                ResetSeeker();
                return;
            }
        }
        else
        {
            _seekerWorldForward = transform.forward;

            if (turnRatePerSecond > _maxSlewRate)
            {
                ResetSeeker();
                return;
            }
        }

        switch (CurrentTargetType)
        {
            case TargetType.Aircraft:
                if (SignalStrength > _signalLockThreshold)
                    _currentLockTime += Time.deltaTime;
                else
                    _currentLockTime -= Time.deltaTime;

                ProcessLockWindow();
                break;

            case TargetType.Sun:
                _currentLockTime += Time.deltaTime * 2.0f;
                ProcessLockWindow();
                break;

            case TargetType.Flare:
                _currentLockTime -= Time.deltaTime * _flareTrackDecayRate;
                _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _lockDuration);
                if (_currentLockTime <= 0f) ResetSeeker();
                break;

            case TargetType.None:
            default:
                _currentLockTime -= Time.deltaTime;
                _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _lockDuration);
                if (_currentLockTime <= 0f && HasLock) ResetSeeker();
                break;
        }
    }

    public void ScanForTargets()
    {
        int combinedMask = _aircraftLayer.value | _flareLayer.value;
        Collider[] contacts = Physics.OverlapSphere(transform.position, _lockRange, combinedMask);

        Transform bestTarget = null;
        float highestSignal = 0f;
        TargetType detectedType = TargetType.None;

        foreach (Collider col in contacts)
        {
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(_seekerWorldForward, dirToTarget);

            if (angle > _seekerFOV / 2f) continue;
            if (Physics.Linecast(transform.position, col.transform.position, _occlusionLayers)) continue;

            float targetSignal = CalculateThermalSignature(col.gameObject, dirToTarget, angle);
            float dist = Vector3.Distance(transform.position, col.transform.position);
            float distanceFactor = Mathf.Clamp01(1f - (dist / _lockRange));

            targetSignal *= distanceFactor;

            if (targetSignal > highestSignal)
            {
                highestSignal = targetSignal;
                bestTarget = col.transform;
                detectedType = (((1 << col.gameObject.layer) & _flareLayer.value) != 0)
                    ? TargetType.Flare
                    : TargetType.Aircraft;
            }
        }

        CurrentTarget = bestTarget;
        SignalStrength = highestSignal;
        CurrentTargetType = detectedType;
    }

    public float GetLockProgress()
    {
        return Mathf.Clamp01(_currentLockTime / _lockDuration); ;
    }

    public void ResetSeeker()
    {
        _currentLockTime = 0f;
        HasLock = false;
        _isUncaged = false;
        SignalStrength = 0f;
        CurrentTarget = null;
        CurrentTargetType = TargetType.None;
        _seekerWorldForward = transform.forward;
    }

    private float CalculateThermalSignature(GameObject contact, Vector3 dirToTarget, float angle)
    {
        float reticleFactor = 1f - (angle / (_seekerFOV / 2f));
        float aspectFactor = 1f;
        float heatMultiplier = 1f;

        if (((1 << contact.layer) & _aircraftLayer.value) != 0)
        {
            float rawDot = Vector3.Dot(contact.transform.forward, dirToTarget);
            float mappedDot = (rawDot + 1f) / 2f;

            aspectFactor = Mathf.Lerp(0.9f, 1.0f, mappedDot);
        }
        else if (((1 << contact.layer) & _flareLayer.value) != 0)
        {
            aspectFactor = 1f;
            heatMultiplier = _flareHeatMultiplier;
        }

        return reticleFactor * aspectFactor * heatMultiplier;
    }

    private void ProcessLockWindow()
    {
        _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _lockDuration);
        if (!HasLock && _currentLockTime >= _lockDuration)
        {
            HasLock = true;
        }
    }
}