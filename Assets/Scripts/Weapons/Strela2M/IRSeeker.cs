using UnityEngine;

public class IRSeeker : MonoBehaviour, IIRSeeker
{
    [SerializeField] private Transform _seekerTransform;

    [SerializeField] private LayerMask _aircraftLayer;
    [SerializeField] private LayerMask _flareLayer;
    [SerializeField] private LayerMask _occlusionLayer;

    [SerializeField] private float _lockrange = 4200f;
    [SerializeField] private float _lockRadius = 3.0f;
    [SerializeField] private float _lockDuration = 3.0f;

    private Transform _currentTarget;

    private bool _hasLock = false;
    private float _currentLockTime = 0f;

    public Transform CurrentTarget { get { return _currentTarget; } }
    public bool HasLock { get { return _hasLock; } }

    public void DoUpdate()
    {
        ScanForTargets();
    }

    public void ScanForTargets()
    {

        if (Physics.SphereCast(_seekerTransform.position, _lockRadius, _seekerTransform.forward, out RaycastHit raycastHit, _lockrange, _aircraftLayer) == true)
        {
            if (_hasLock == false)
            {
                _currentLockTime += Time.deltaTime;

                if (_currentLockTime >= _lockDuration)
                {
                    _currentTarget = raycastHit.transform;
                    _hasLock = true;
                }
            }
        }
        else
        {
            if (_hasLock == true)
            {
                ResetSeeker();
            }
        }
    }

    public float GetLockProgress()
    {
        return Mathf.Clamp01(_currentLockTime / _lockDuration);
    }

    public void ResetSeeker()
    {
        _hasLock = false;
        _currentLockTime = 0f;
        _currentTarget = null;
    }
}
