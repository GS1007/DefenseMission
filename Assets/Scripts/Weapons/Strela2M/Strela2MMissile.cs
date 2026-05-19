using System.Collections;
using UnityEngine;

public class Strela2MMissile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody _rb;
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
    private float _guidanceStartTime;
    private Vector3 _lastTargetPos;
    private Transform _target;
    private TargetType _targetType;

    public Strela2MSeeker Seeker { get { return _seeker; } }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = Vector3.zero;

        if (_target != null)
        {
            targetVelocity = (_target.position - _lastTargetPos) / Time.fixedDeltaTime;
            _lastTargetPos = _target.position;
        }

        if (!_motorIgnited) return;

        if (_target != null && Time.time > _guidanceStartTime + 0.1f)
        {
            if (_targetType == TargetType.Sun)
                ApplySunGuidance();
            else
                ApplyProportionalNavigation(targetVelocity);
        }
        else if (_rb.linearVelocity.sqrMagnitude > 1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }

        ApplyThrust();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isAirborne == true)
        {
            Debug.Log($"Hit: {collision.gameObject.name}");

            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.ReceiveDamage();

            Destroy(gameObject);
        }
    }

    public void Launch(Transform target)
    {
        transform.parent = null;
        _isAirborne = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;

        _target = target;
        _targetType = _seeker.CurrentTargetType;
        _targetType = TargetType.Aircraft;

        if (_target != null) _lastTargetPos = _target.position;

        _rb.AddForce(transform.forward * _ejectionForce, ForceMode.VelocityChange);
        StartCoroutine(IgniteSustainer(_sustainerDelay));
    }

    private void ApplyProportionalNavigation(Vector3 safeTargetVelocity)
    {
        float distance = Vector3.Distance(transform.position, _target.position);
        float currentSpeed = Mathf.Max(_rb.linearVelocity.magnitude, 10f);
        float timeToTarget = Mathf.Min(distance / currentSpeed, 3f);

        Vector3 interceptPoint = _target.position + (safeTargetVelocity * timeToTarget);
        Vector3 desiredDirection = (interceptPoint - transform.position).normalized;

        float turnSpeed = _navigationConstant * 10f;

        _rb.linearVelocity = Vector3.RotateTowards(
            _rb.linearVelocity,
            desiredDirection * _rb.linearVelocity.magnitude,
            turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime,
            0f
        );

        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(_rb.linearVelocity);
        }
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
        {
            _rb.AddForce(transform.forward * _sustainerThrust, ForceMode.Acceleration);
        }
    }

    private IEnumerator IgniteSustainer(float delay)
    {
        yield return new WaitForSeconds(delay);

        _motorIgnited = true;
        _rb.useGravity = false;
        _guidanceStartTime = Time.time;

        Destroy(gameObject, _selfDestructTime);
    }
}