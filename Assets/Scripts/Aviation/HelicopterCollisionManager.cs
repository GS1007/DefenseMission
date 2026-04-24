using UnityEngine;
using UnityEngine.Events;

public class HelicopterCollisionManager : MonoBehaviour, IDamageable, IAircraftTarget
{
    [SerializeField] private GameObject _damagedAircraftPrefab;

    [SerializeField] private Transform _sweetSpot;
    [SerializeField] private Transform _damagePoint;

    [SerializeField] private Rigidbody _rb;

    [Header("Events")]

    [SerializeField] private UnityEvent OnDamageReceive;

    private Transform _currentHitPoint;

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

    public Transform GetSweetSpot()
    {
        _currentHitPoint = _sweetSpot;

        return _currentHitPoint;
    }

    public Transform GetDamagePoint()
    {
        _currentHitPoint = _damagePoint;

        return _currentHitPoint;
    }

    public void ReceiveDamage()
    {
        if (_currentHitPoint == _damagePoint)
        {
            _rb.isKinematic = false;
            _partiallyDamaged = true;
        }
        else
        {
            OnDamageReceive?.Invoke();

            Instantiate(_damagedAircraftPrefab, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    private void HandlePartialDamage()
    {
        _rotationTimer += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(_currentRotation, _targetRotation, _rotationTimer / _rotationTime);

        if (_rotationTimer >= _rotationTime)
        {
            _rotationTimer = 0f;
            SetDamageRotation();
        }
    }

    private void SetDamageRotation()
    {
        _currentRotation = transform.rotation;
        _targetRotation = Quaternion.Euler(0f, Random.Range(60, 120f), Random.Range(-35f, 35f));
    }
}
