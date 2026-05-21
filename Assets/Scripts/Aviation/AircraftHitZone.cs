using UnityEngine;

public class AircraftHitZone : MonoBehaviour
{
    [SerializeField] private AircraftCollisionManager _aircraftCollisionManager;

    [SerializeField] private AircraftHitZoneType _aircraftHitZoneType;

    private IDamageable _damageable;

    public IDamageable GetDamageable { get { return _damageable; } }

    public AircraftHitZoneType HitZoneType { get { return _aircraftHitZoneType; } }

    private void Start()
    {
        if (_aircraftCollisionManager is IDamageable damageable)
        {
            _damageable = damageable;
        }
    }
}
