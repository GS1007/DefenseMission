using System.Collections;
using UnityEngine;

public class Strela2MMissile : MonoBehaviour
{
    [Header("Missile Components")]
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private float _lifeTimeAftertargetHit = 0f;

    [Header("Effects")]
    [SerializeField] private GameObject _explosionPrefab;

    [Header("Flight Settings")]
    [SerializeField] private float _throwSpeed = 0f;
    [SerializeField] private float _accelerationTime = 0f;
    [SerializeField] private float _flightSpeed = 0f;
    [SerializeField] private float _navigationConstant = 4.0f; // N-factor (3-5 is ideal)
    [SerializeField] private float _fuseDistance = 3.0f;
    [SerializeField] private float _lifetime = 10f;

    private WaitForSeconds _accelerationDelay;

    private Transform _target;
    private Vector3 _lastLineOfSight;

    private bool _isEngineActive = false;
    private bool _isSeeking = false;
    private bool _exploded = false;

    private void Awake()
    {
        _accelerationDelay = new WaitForSeconds(_accelerationTime);
    }

    private void FixedUpdate()
    {
        if (_exploded == true) return;

        if (_isEngineActive == true)
        {
            _rb.linearVelocity = transform.forward * _flightSpeed;
        }

        if (!_isSeeking || _target == null) return;

        Vector3 currentLineOfSight = (_target.position - transform.position).normalized;
        Vector3 rotationAxis = Vector3.Cross(_lastLineOfSight, currentLineOfSight);
        float relativeRotation = Mathf.Asin(rotationAxis.magnitude);

        if (rotationAxis.magnitude > 0.001f)
        {
            float steerAngle = relativeRotation * _navigationConstant;
            Quaternion steerRotation = Quaternion.AngleAxis(steerAngle * Mathf.Rad2Deg, rotationAxis.normalized);
            _rb.MoveRotation(steerRotation * transform.rotation);
        }

        _lastLineOfSight = currentLineOfSight;

        if (Vector3.Distance(transform.position, _target.position) < _fuseDistance)
        {
            Explode();
            _exploded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_exploded == false)
        {
            if (!_isSeeking && collision.transform == _target) return;

            Explode();
            _exploded = true;
        }
    }

    public void LaunchTowardsTarget(Transform targetTransform)
    {
        ThrowMissile();
        StartCoroutine(AccelerateMissile(targetTransform, true));
    }

    public void MissTarget()
    {
        ThrowMissile();
        StartCoroutine(AccelerateMissile(null, false));
    }

    private void ThrowMissile()
    {
        _rb.linearVelocity = transform.forward * _throwSpeed;
    }

    private void Explode()
    {
        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, _target.position, Quaternion.identity);
        }

        _target.parent.GetComponent<IDamageable>().ReceiveDamage();

        _rb.linearVelocity = Vector3.zero;
        _boxCollider.enabled = false;
        _meshRenderer.enabled = false;

        Destroy(gameObject, _lifeTimeAftertargetHit);
    }

    private IEnumerator AccelerateMissile(Transform targetTransform, bool shouldHitTarget)
    {
        yield return _accelerationDelay;

        if (shouldHitTarget == true)
        {
            _target = targetTransform;
            _lastLineOfSight = (_target.position - transform.position).normalized;
            _isSeeking = true;
        }
        else
        {
            _isSeeking = false;

            float veerAngleX = Random.Range(-10f, 10f);
            float veerAngleY = Random.Range(-10f, 10f);
            transform.Rotate(veerAngleX, veerAngleY, 0f);
        }

        _isEngineActive = true;

        Destroy(gameObject, _lifetime);
    }
}
