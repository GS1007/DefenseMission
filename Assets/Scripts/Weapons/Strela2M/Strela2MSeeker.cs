using UnityEngine;

public class Strela2MSeeker : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float _lockRange = 4000f;
    [SerializeField] private float _seekerFOV = 1.8f;
    [SerializeField] private LayerMask _aircraftLayer;
    [SerializeField] private LayerMask _flareLayer;
    [SerializeField] private LayerMask _occlusionLayers;

    [Header("Design Spec Limits")]
    [SerializeField] private float _lockDuration = 1.2f;
    [SerializeField] private float _maxTurnRate = 1.5f;
    [SerializeField] private float _flareHeatMultiplier = 3.0f;
    [SerializeField] private float _gimbalLimit = 40f;

    private float _currentLockTime;
    private Vector3 _lastForward;
    private Vector3 _seekerWorldForward;
    private bool _isUncaged;
    private Transform _sunTransform;
    private Transform _seekerTransform;

    public Transform CurrentTarget { get; private set; }
    public float SignalStrength { get; private set; }
    public TargetType CurrentTargetType { get; private set; }
    public bool HasLock { get; private set; }
    public float LockProgress => Mathf.Clamp01(_currentLockTime / _lockDuration);

    public void Init(Transform seekerTransform)
    {
        _seekerTransform = seekerTransform;
        _sunTransform = GameObject.FindGameObjectWithTag("Sun").transform;
        _seekerWorldForward = _seekerTransform.forward;
        _lastForward = _seekerTransform.forward;
    }

    public void DoUpdate()
    {
        float frameTurnSpeed = Vector3.Angle(_lastForward, _seekerTransform.forward);
        _lastForward = transform.forward;

        if (_isUncaged && CurrentTarget != null)
        {
            Vector3 dirToTarget = (CurrentTarget.position - _seekerTransform.position).normalized;
            _seekerWorldForward = Vector3.RotateTowards(
                _seekerWorldForward,
                dirToTarget,
                _maxTurnRate * Mathf.Deg2Rad * Time.deltaTime,
                0f);

            float gimbalAngle = Vector3.Angle(_seekerTransform.forward, _seekerWorldForward);

            if (gimbalAngle > _gimbalLimit)
            {
                ResetSeeker();
                return;
            }
        }
        else
        {
            _seekerWorldForward = _seekerTransform.forward;

            if (frameTurnSpeed > _maxTurnRate)
            {
                ResetSeeker();
                return;
            }
        }

        ScanForTargets();

        if (CurrentTargetType == TargetType.Aircraft)
        {
            if (SignalStrength > 0.6f)
                _currentLockTime += Time.deltaTime;
            else
                _currentLockTime -= Time.deltaTime;

            _currentLockTime = Mathf.Clamp(_currentLockTime, 0f, _lockDuration);

            if (!HasLock && _currentLockTime >= _lockDuration)
            {
                HasLock = true;
                _isUncaged = true;
                _seekerWorldForward = _seekerTransform.forward;
            }
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
        _seekerWorldForward = _seekerTransform.forward;
    }

    private void ScanForTargets()
    {
        int combinedMask = _aircraftLayer.value | _flareLayer.value;
        Collider[] contacts = Physics.OverlapSphere(_seekerTransform.position, _lockRange, combinedMask);

        Transform bestTarget = null;
        float highestSignal = 0f;
        TargetType detectedType = TargetType.None;

        foreach (Collider col in contacts)
        {
            Vector3 dirToTarget = (col.transform.position - _seekerTransform.position).normalized;

            float angle = Vector3.Angle(_seekerWorldForward, dirToTarget);

            if (angle > _seekerFOV / 2f) continue;

            if (Physics.Linecast(_seekerTransform.position, col.transform.position, _occlusionLayers)) continue;

            float targetSignal = CalculateThermalSignature(col.gameObject, dirToTarget, angle);

            float dist = Vector3.Distance(_seekerTransform.position, col.transform.position);
            targetSignal *= Mathf.Clamp01(1f - (dist / _lockRange));

            if (targetSignal > highestSignal)
            {
                highestSignal = targetSignal;
                bestTarget = col.transform;
                detectedType = (((1 << col.gameObject.layer) & _flareLayer.value) != 0)
                    ? TargetType.Cloud
                    : TargetType.Aircraft;
            }
        }

        Vector3 dirToSun = (_sunTransform.position - _seekerTransform.position).normalized;
        float angleToSun = Vector3.Angle(_seekerWorldForward, dirToSun);

        // if (angleToSun < 30f)
        // {
        //     float sunSignal = 1.1f;
        //     if (sunSignal > highestSignal)
        //     {
        //         highestSignal = sunSignal;
        //         bestTarget = _sunTransform;
        //         detectedType = TargetType.Sun;
        //     }
        // }

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
            aspectFactor = Mathf.Lerp(0.7f, 1.0f, mappedDot);
        }
        else if (((1 << contact.layer) & _flareLayer.value) != 0)
        {
            aspectFactor = 1f;
            heatMultiplier = _flareHeatMultiplier;
        }

        return reticleFactor * aspectFactor * heatMultiplier;
    }
}