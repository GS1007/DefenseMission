using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class JetCollisionManager : MonoBehaviour, IDamageable, IAircraftTarget
{
    [SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private Transform _sweetSpot;
    [SerializeField] private Transform _damagePoint;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private CrashBehaviorSO _crashBehavior;
    [SerializeField] private SplineAnimate _splineAnimate;

    [SerializeField] private UnityEvent OnDamageReceive;

    private bool _hitGround = false;
    private bool _isCrashing = false;

    private Coroutine _activeCrashRoutine;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") == true && _isCrashing == true && _hitGround == false)
        {
            HandleGroundImpact(collision);
        }
    }

    public Transform GetSweetSpot()
    {
        return _sweetSpot;
    }

    public Transform GetDamagePoint()
    {
        return _damagePoint;
    }

    public void ReceiveDamage()
    {
        OnDamageReceive?.Invoke();

        if (_isCrashing == true || _crashBehavior == null) return;

        _isCrashing = true;

        _splineAnimate.enabled = false;

        _rb.isKinematic = false;
        _rb.useGravity = true;


        _activeCrashRoutine = StartCoroutine(_crashBehavior.CrashRoutine(_rb, () => _hitGround));

        Debug.Log("Damage Taken");
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

        Destroy(gameObject, 10f);
    }
}
