using UnityEngine;

public class AircraftDestructor : MonoBehaviour
{
    [Header("Explosion Physics")]

    [SerializeField] private Rigidbody[] _parts;

    [SerializeField] private float _explosionForce = 0f;
    [SerializeField] private float _explosionRadius = 0f;
    [SerializeField] private float _upwardModifier = 0f;


    [Header("Crash Components")]

    [SerializeField] private GameObject _aircraft;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private CrashBehaviorSO _crashBehavior;

    [SerializeField] private float _lifeTimeAfterCrash = 0f;

    private bool _hitGround = false;
    private bool _isCrashing = false;

    private Coroutine _activeCrashRoutine;

    private void Start()
    {
        ShatterAircraft();
        HandleBodyDamage();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") == true && _isCrashing == true && _hitGround == false)
        {
            HandleGroundImpact(collision);
        }
    }

    private void ShatterAircraft()
    {
        foreach (Rigidbody rb in _parts)
        {
            rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, _upwardModifier, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * _explosionForce, ForceMode.Impulse);
        }
    }

    private void HandleBodyDamage()
    {
        if (_isCrashing == true || _crashBehavior == null) return;

        _isCrashing = true;


        _activeCrashRoutine = StartCoroutine(_crashBehavior.CrashRoutine(_rb, () => _hitGround));
    }

    private void HandleGroundImpact(Collision collision)
    {
        _hitGround = true;

        if (_activeCrashRoutine != null)
        {
            StopCoroutine(_activeCrashRoutine);
        }

        if (_explosionPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(_explosionPrefab, contact.point, Quaternion.identity);
        }

        _rb.angularVelocity = Vector3.zero;
        _rb.linearVelocity = _rb.linearVelocity * 0.2f;

        Destroy(_aircraft, _lifeTimeAfterCrash);
    }
}
