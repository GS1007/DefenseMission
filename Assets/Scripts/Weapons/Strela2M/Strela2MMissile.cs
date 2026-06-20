using System.Collections;
using UnityEngine;

public class Strela2MMissile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Strela2MSeeker _seeker;

    [Header("Flight Dynamics")]
    [SerializeField] private float _ejectionForce = 15f;
    [SerializeField] private float _sustainerDelay = 0.4f;
    [SerializeField] private float _sustainerThrust = 45f;
    [SerializeField] private float _maxSpeed = 430f;
    [SerializeField] private float _navigationConstant = 4.0f;
    [SerializeField] private float _selfDestructTime = 14f;

    private bool _isAirborne = false;
    private bool _motorIgnited = false;
    private bool _isCriticalHit = false;

    private float _guidanceStartTime;

    private Vector3 _lastLosVector;

    private Transform _target;
    private TargetType _targetType;

    public Strela2MSeeker Seeker => _seeker;

    private void FixedUpdate()
    {
        if (!_isAirborne) return;

        _seeker.DoUpdate();

        if (_target != _seeker.CurrentTarget && _seeker.CurrentTarget != null)
            _lastLosVector = (_seeker.CurrentTarget.position - transform.position).normalized;

        _target = _seeker.CurrentTarget;
        _targetType = _seeker.CurrentTargetType;

        if (!_motorIgnited) return;

        if (_target == null || _targetType == TargetType.None)
        {
            if (_rb.linearVelocity.sqrMagnitude > 1f)
                transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);

            ApplyThrust();

            return;
        }

        if (Time.time > _guidanceStartTime + 0.1f)
        {
            if (_targetType == TargetType.Sun)
                ApplySunGuidance();
            else
                ApplyProportionalNavigation();
        }
        else if (_rb.linearVelocity.sqrMagnitude > 1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }

        ApplyThrust();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isAirborne) return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if (_isCriticalHit)
                damageable.ReceiveCriticalDamage();
            else
                damageable.ReceiveDamage();
        }

        Destroy(gameObject);
    }

    public void Launch(bool isCriticalHit)
    {
        _meshRenderer.enabled = true;

        transform.parent = null;
        _isAirborne = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;

        _target = _seeker.CurrentTarget;
        _targetType = _seeker.CurrentTargetType;
        _isCriticalHit = isCriticalHit;

        if (_target != null)
            _lastLosVector = (_target.position - transform.position).normalized;

        _rb.AddForce(transform.forward * _ejectionForce, ForceMode.VelocityChange);
        StartCoroutine(IgniteSustainer(_sustainerDelay));
    }

    private void ApplyProportionalNavigation()
    {
        Vector3 currentLos = (_target.position - transform.position).normalized;

        Vector3 losRotationAxis = Vector3.Cross(_lastLosVector, currentLos);
        float losAngleDelta = Vector3.Angle(_lastLosVector, currentLos) * Mathf.Deg2Rad;
        Vector3 angularVelocityLos = (losRotationAxis.normalized * losAngleDelta) / Time.fixedDeltaTime;

        _lastLosVector = currentLos;

        Vector3 missileVelocity = _rb.linearVelocity;
        Vector3 lateralAccelerationCommand = Vector3.Cross(angularVelocityLos * _navigationConstant, missileVelocity);

        _rb.linearVelocity += lateralAccelerationCommand * Time.fixedDeltaTime;

        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
    }

    private void ApplySunGuidance()
    {
        Vector3 sunDir = (_target.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(sunDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.fixedDeltaTime);
        _rb.linearVelocity = transform.forward * _rb.linearVelocity.magnitude;
    }

    private void ApplyThrust()
    {
        if (_rb.linearVelocity.magnitude < _maxSpeed)
            _rb.AddForce(transform.forward * _sustainerThrust, ForceMode.Acceleration);

        if (_rb.linearVelocity.magnitude > _maxSpeed)
            _rb.linearVelocity = _rb.linearVelocity.normalized * _maxSpeed;
    }

    private IEnumerator IgniteSustainer(float delay)
    {
        yield return new WaitForSeconds(delay);

        _rb.useGravity = false;
        _motorIgnited = true;

        if (_target != null)
            _lastLosVector = (_target.position - transform.position).normalized;

        _guidanceStartTime = Time.time;
        Destroy(gameObject, _selfDestructTime);
    }
}