using System;
using UnityEngine;
using UnityEngine.Events;

public class AircraftCollisionManager : MonoBehaviour, IDamageable, IAircraftTarget
{
    public static event Action<IAircraftTarget, bool> DamagedReceived;

    [SerializeField] private GameObject _damagedAircraftPrefab;
    [SerializeField] private GameObject _exposionVFX;

    [SerializeField] private Transform _aircraft;
    [SerializeField] private Transform _criticalPoint;
    [SerializeField] private Transform _partialPoint;

    [SerializeField] private AircraftType _aircraftTyoe;

    [Header("Events")]

    [SerializeField] private UnityEvent OnCriticalDamageReceive;
    [SerializeField] private UnityEvent OnPartialDamageReceive;


    public void ReceiveDamage()
    {
        DamagedReceived?.Invoke(this, false);
        Instantiate(_exposionVFX, transform);
    }

    public void ReceiveCriticalDamage()
    {
        OnCriticalDamageReceive?.Invoke();
        DamagedReceived?.Invoke(this, true);

        Instantiate(_damagedAircraftPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public AircraftType GetAircraftType()
    {
        return _aircraftTyoe;
    }
}
