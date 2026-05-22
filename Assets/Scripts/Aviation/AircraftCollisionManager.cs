using System;
using UnityEngine;
using UnityEngine.Events;

public class AircraftCollisionManager : MonoBehaviour, IDamageable, IAircraftTarget
{
    public static event Action<bool> DamagedReceived;

    [SerializeField] private GameObject _damagedAircraftPrefab;
    [SerializeField] private GameObject _exposionVFX;

    [SerializeField] private Transform _aircraft;
    [SerializeField] private Transform _criticalPoint;
    [SerializeField] private Transform _partialPoint;

    [SerializeField] private AircraftType _aircraftTyoe;

    [Header("Events")]

    [SerializeField] private UnityEvent OnDamageReceive;

    private bool _partiallyDamaged = false;

    private float _rotationTimer = 0f;
    private float _rotationTime = 0.75f;

    private Quaternion _currentRotation;
    private Quaternion _targetRotation;

    private void Start()
    {
        SetDamageRotation();
    }

    private void Update()
    {
        if (_partiallyDamaged == true)
        {
            HandlePartialDamage();
        }
    }

    public void ReceiveDamage()
    {
        _partiallyDamaged = true;
        DamagedReceived?.Invoke(false);
        Instantiate(_exposionVFX, transform);
    }

    public void ReceiveCriticalDamage()
    {
        OnDamageReceive?.Invoke();
        DamagedReceived?.Invoke(true);

        Instantiate(_damagedAircraftPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public Transform GetPartialPoint()
    {
        return _partialPoint;
    }

    public Transform GetCriticalPoint()
    {
        return _criticalPoint;
    }

    private void HandlePartialDamage()
    {
        _rotationTimer += Time.deltaTime;

        _aircraft.rotation = Quaternion.Slerp(_currentRotation, _targetRotation, _rotationTimer / _rotationTime);

        if (_rotationTimer >= _rotationTime)
        {
            _rotationTimer = 0f;
            SetDamageRotation();
        }
    }

    private void SetDamageRotation()
    {
        _currentRotation = _aircraft.rotation;
        _targetRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(60, 120f), UnityEngine.Random.Range(-35f, 35f));
    }

    public AircraftType GetAircraftType()
    {
        return _aircraftTyoe;
    }
}
