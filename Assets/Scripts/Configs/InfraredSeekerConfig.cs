using UnityEngine;

[CreateAssetMenu(fileName = "NewIRSeekerConfig", menuName = "Weapons/Seekers/Infrared")]
public class InfraredSeekerConfig : ScriptableObject
{
    [Header("Detection")]
    [SerializeField] private float _lockRange = 4200f;
    [SerializeField] private float _fieldOfView = 1.5f;
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private LayerMask _occlusionLayers;

    [Header("Tracking Parameters")]
    [SerializeField] private float _lockDuration = 1.8f;
    [SerializeField] private float _maxSlewRate = 60f;
    [SerializeField] private float _signalLockThreshold = 0.2f;
    [SerializeField] private float _gimbalLimit = 25f;
    [SerializeField] private float _trackrate = 11f;

    [Header("Environment Targets")]
    [SerializeField] private float _sunAngle = 25f;
    [SerializeField] private float _sunThermalSignature = 5.0f;
    [SerializeField] private string _sunTag = "Sun";

    public float LockRange { get { return _lockRange; } }
    public float FieldOfView { get { return _fieldOfView; } }
    public LayerMask TargetLayers { get { return _targetLayers; } }
    public LayerMask OcclusionLayers { get { return _occlusionLayers; } }
    public float LockDuration { get { return _lockDuration; } }
    public float MaxSlewRate { get { return _maxSlewRate; } }
    public float SignalLockThreshold { get { return _signalLockThreshold; } }
    public float GimbalLimit { get { return _gimbalLimit; } }
    public float Trackrate { get { return _trackrate; } }
    public float SunAngle { get { return _sunAngle; } }
    public float SunThermalSignature { get { return _sunThermalSignature; } }
    public string SunTag { get { return _sunTag; } }
}
