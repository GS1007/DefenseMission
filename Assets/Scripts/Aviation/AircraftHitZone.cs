using UnityEngine;

public class AircraftHitZone : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _damageableBehaviour;

    [SerializeField] private AircraftHitZoneType _aircraftHitZoneType;

    private IDamageable _damageable;

    public IDamageable GetDamageable { get { return _damageable; } }

    public AircraftHitZoneType HitZoneType { get { return _aircraftHitZoneType; } }

    private void Start()
    {
        if (_damageableBehaviour is IDamageable damageable)
        {
            _damageable = damageable;
        }
    }
}
