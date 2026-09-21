using UnityEngine;

public class InfraredSeeker : MonoBehaviour, ISeeker
{
    [SerializeField] private InfraredSeekerConfig _config;

    private readonly Collider[] _scanResults = new Collider[32];

    private bool _isUncaged;
    private bool _isTrackingActive;
    private float _currentLockTime;
    private Transform _sunTransform;
    private Vector3 _lastForward;
    private Vector3 _seekerWorldForward;

    public Transform CurrentTarget { get; private set; }
    public TargetType CurrentTargetType { get; private set; }
    public bool HasLock { get; private set; }
    public float SignalStrength { get; private set; }
    public float TargetLockTime { get; private set; }
    public float LockProgress => Mathf.Clamp01(_currentLockTime / _config.LockDuration);

    private void Awake()
    {
        GameObject sunObj = GameObject.FindGameObjectWithTag(_config.SunTag);
        if (sunObj != null) _sunTransform = sunObj.transform;
    }

    private void Update()
    {
        if (!_isTrackingActive) return;

        float frameTurnAngle = Vector3.Angle(_lastForward, transform.forward);
        float turnRatePerSecond = frameTurnAngle / Time.deltaTime;
        _lastForward = transform.forward;

        PerformScan();

        if (_isUncaged && CurrentTarget != null)
        {
            Vector3 dirToTarget = (CurrentTarget.position - transform.position).normalized;
            _seekerWorldForward = Vector3.RotateTowards(
                _seekerWorldForward,
                dirToTarget,
                _config.Trackrate * Mathf.Deg2Rad * Time.deltaTime,
                0f);

            float angleLimit = _config.GimbalLimit;

            if (CurrentTargetType == TargetType.Aircraft)
            {
                angleLimit = _config.GimbalLimit;
            }
            else if (CurrentTargetType == TargetType.Cloud)
            {
                angleLimit = CurrentTarget.GetComponent<OcclusionObjectAngleConfig>().DetectionAngle;
            }
            else
            {
                angleLimit = _config.SunAngle;
            }

            if (Vector3.Angle(transform.forward, _seekerWorldForward) > angleLimit)
            {
                ResetSeeker();
                return;
            }
        }
        else
        {
            if (!HasLock && !_isUncaged)
                _seekerWorldForward = transform.forward;

            if (turnRatePerSecond > _config.MaxSlewRate)
            {
                ResetSeeker();
                return;
            }
        }

        switch (CurrentTargetType)
        {
            case TargetType.Aircraft:
                _currentLockTime += (SignalStrength > _config.SignalLockThreshold) ? Time.deltaTime : -Time.deltaTime;
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

    public void SetTrackingActive(bool isActive)
    {
        _isTrackingActive = isActive;

        if (isActive == true)
        {
            _seekerWorldForward = transform.forward;
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

    private void PerformScan()
    {
        int combinedMask = _config.TargetLayers | _config.OcclusionLayers;
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _config.LockRange, _scanResults, combinedMask);

        Transform bestTarget = null;
        float highestSignal = 0f;
        TargetType detectedType = TargetType.None;

        for(int i = 0; i < hitCount; i++)
        {
            Collider col = _scanResults[i];
            float dist = Vector3.Distance(transform.position, col.transform.position);
            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(_seekerWorldForward, dirToTarget);
            bool isCloud = ((1 << col.gameObject.layer) & _config.OcclusionLayers.value) != 0;

            if (isCloud)
            {
                OcclusionObjectAngleConfig occlusionObjectAngleConfig = col.GetComponent<OcclusionObjectAngleConfig>();

                if (angle <= occlusionObjectAngleConfig.DetectionAngle)
                {
                    bestTarget = col.transform;
                    highestSignal = 1f;
                    detectedType = TargetType.Cloud;
                    break;
                }
            }

            if (angle > _config.FieldOfView / 2f)
                continue;

            float targetSignal = CalculateThermalSignature(col.gameObject, dirToTarget, angle);
            targetSignal *= Mathf.Clamp01(1f - (dist / _config.LockRange));

            if (targetSignal > highestSignal)
            {
                highestSignal = targetSignal;
                bestTarget = col.transform;
                detectedType = TargetType.Aircraft;
            }

            if (_sunTransform != null)
            {
                Vector3 dirToSun = (_sunTransform.position - transform.position).normalized;
                float sunAngle = Vector3.Angle(_seekerWorldForward, dirToSun);

                if (sunAngle <= _config.SunAngle)
                {
                    float sunSignal = (1f - (sunAngle / (_config.SunAngle / 2f))) * _config.SunThermalSignature;

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
    }

    private float CalculateThermalSignature(GameObject contact, Vector3 dirToTarget, float angle)
    {
        float reticleFactor = 1f - (angle / (_config.FieldOfView / 2f));
        float aspectFactor = 1f;
        float heatMultiplier = 1f;

        if (((1 << contact.layer) & _config.TargetLayers.value) != 0)
        {
            float rawDot = Vector3.Dot(contact.transform.forward, dirToTarget);
            float mappedDot = (rawDot + 1f) / 2f;
            aspectFactor = Mathf.Lerp(0.9f, 1.0f, Mathf.Pow(mappedDot, 2f));
        }

        return reticleFactor * aspectFactor * heatMultiplier;
    }

    private void ProcessLockWindow()
    {
        _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _config.LockDuration);

        if (!HasLock && _currentLockTime >= _config.LockDuration)
        {
            _isUncaged = true;
            HasLock = true;
            TargetLockTime = Time.time;
        }

        if (HasLock && _currentLockTime <= 0f) HasLock = false;
    }
}
