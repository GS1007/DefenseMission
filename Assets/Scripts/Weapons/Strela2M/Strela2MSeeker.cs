using UnityEngine;

public class Strela2MSeeker : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float _lockRange = 4200;
    [SerializeField] private float _seekerFOV = 2.0f;
    [SerializeField] private float _cloudAngle = 20f;
    [SerializeField] private float _sunAngle = 25f;

    [SerializeField] private LayerMask _aircraftLayer;
    [SerializeField] private LayerMask _occlusionLayers;

    [Header("Design Spec Limits")]
    [SerializeField] private float _lockDuration = 1.8f;
    [SerializeField] private float _maxSlewRate = 60f;
    [SerializeField] private float _seekerTrackRate = 11f;

    [Header("Thermal & Countermeasures")]
    [SerializeField] private float _gimbalLimit = 25f;
    [SerializeField] private float _signalLockThreshold = 0.2f;
    [SerializeField] private float _sunThermalSignature = 5.0f;

    private float _currentLockTime;
    private Vector3 _lastForward;
    private Vector3 _seekerWorldForward;
    private bool _isUncaged;
    private Transform _sunTransform;

    public Transform CurrentTarget { get; private set; }
    public float SignalStrength { get; private set; }
    public TargetType CurrentTargetType { get; private set; }
    public bool HasLock { get; private set; }

    public float LockProgress => Mathf.Clamp01(_currentLockTime / _lockDuration);

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

        if (_isUncaged && CurrentTarget != null)
        {
            Vector3 dirToTarget = (CurrentTarget.position - transform.position).normalized;
            _seekerWorldForward = Vector3.RotateTowards(
                _seekerWorldForward,
                dirToTarget,
                _seekerTrackRate * Mathf.Deg2Rad * Time.deltaTime,
                0f);

            if (Vector3.Angle(transform.forward, _seekerWorldForward) > _gimbalLimit)
            {
                ResetSeeker();
                return;
            }
        }
        else
        {
            if (!HasLock && !_isUncaged)
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
                _currentLockTime += (SignalStrength > _signalLockThreshold) ? Time.deltaTime : -Time.deltaTime;
                ProcessLockWindow();

                if (!HasLock && _currentLockTime <= 0f) ResetSeeker();
                break;

            case TargetType.Sun:
                _currentLockTime += Time.deltaTime * 2.0f;
                ProcessLockWindow();
                break;

            case TargetType.Cloud:
                _currentLockTime += Time.deltaTime * 2.0f;
                ProcessLockWindow();
                break;

            default:
                if (HasLock == true)
                {
                    ResetSeeker();
                }
                else
                {
                    _currentLockTime = Mathf.Max(0f, _currentLockTime - Time.deltaTime);
                }
                break;
        }
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

    private void ScanForTargets()
    {
        int combinedMask = _aircraftLayer.value | _occlusionLayers.value;
        Collider[] contacts = Physics.OverlapSphere(transform.position, _lockRange, combinedMask);

        Transform bestTarget = null;
        float highestSignal = 0f;
        TargetType detectedType = TargetType.None;

        foreach (Collider col in contacts)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);

            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(_seekerWorldForward, dirToTarget);

            bool isCloud = ((1 << col.gameObject.layer) & _occlusionLayers.value) != 0;

            if (isCloud && angle <= _cloudAngle)
            {
                bestTarget = col.transform;
                highestSignal = 1f;
                detectedType = TargetType.Cloud;
                break;
            }

            if (angle > _seekerFOV / 2f)
                continue;

            float targetSignal = CalculateThermalSignature(col.gameObject, dirToTarget, angle);
            targetSignal *= Mathf.Clamp01(1f - (dist / _lockRange));

            if (targetSignal > highestSignal)
            {
                highestSignal = targetSignal;
                bestTarget = col.transform;
                detectedType = TargetType.Aircraft;
            }
        }

        if (_sunTransform != null)
        {
            Vector3 dirToSun = (_sunTransform.position - transform.position).normalized;
            float sunAngle = Vector3.Angle(_seekerWorldForward, dirToSun);

            if (sunAngle <= _sunAngle)
            {
                float sunSignal = (1f - (sunAngle / (_sunAngle / 2f))) * _sunThermalSignature;

                if (sunSignal > highestSignal)
                {
                    highestSignal = sunSignal;
                    bestTarget = _sunTransform;
                    detectedType = TargetType.Sun;
                }
            }
        }

        CurrentTarget = bestTarget;
        SignalStrength = highestSignal;
        CurrentTargetType = detectedType;
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
            aspectFactor = Mathf.Lerp(0.9f, 1.0f, Mathf.Pow(mappedDot, 2f));
        }

        return reticleFactor * aspectFactor * heatMultiplier;
    }

    private void ProcessLockWindow()
    {
        _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _lockDuration);

        if (!HasLock && _currentLockTime >= _lockDuration)
        {
            _isUncaged = true;
            HasLock = true;
        }

        if (HasLock && _currentLockTime <= 0f) HasLock = false;
    }
}