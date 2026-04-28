using UnityEngine;

public class Strela2MAimController : MonoBehaviour
{
    [SerializeField] private Transform _headView;
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Transform _strela;
    [SerializeField] private Transform _aimingPoint;
    [SerializeField] private Transform _tempAimingPoint;

    [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRHolding;
    [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRAiming;

    [Header("Aiming")]

    [SerializeField] private LayerMask _firstAimingPoint;
    [SerializeField] private LayerMask _secondAimingPoint;

    [SerializeField] private float _aimRange = 0f;

    private bool _isAiming = false;

    [SerializeField] private bool _isTracking = false;

    private Vector3 __aimingPointStartPosition = Vector3.zero;

    private void Start()
    {
        __aimingPointStartPosition = _aimingPoint.localPosition;
    }

    private void Update()
    {
        //if (CheckForAiming() == true)
        //{
        //    if (_isAiming == false)
        //    {
        //        AttachStrelaToCamera();
        //        _isAiming = true;
        //    }
        //}
        //else
        //{
        //    if (_isAiming == true)
        //    {
        //        AttachStrelaToController();
        //        _isAiming = false;
        //    }
        //}

        if (_isTracking == true)
        {
            TrackPoition();

            _isTracking = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AimingPoint") == true)
        {
            AttachStrelaToCamera();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AimingPoint") == true)
        {
            AttachStrelaToController();
        }
    }

    private void AttachStrelaToCamera()
    {
        Debug.Log("Aiming");
        _strela.parent = _headView;
        //_strela.localPosition = _objectTransformSettingsVRAiming.Position;
        //_strela.localRotation = Quaternion.Euler(_objectTransformSettingsVRAiming.Rotation);
    }

    private void AttachStrelaToController()
    {
        _strela.parent = _weaponHolder;
    }

    private bool CheckForAiming()
    {
        return Physics.Raycast(_headView.position, _headView.forward, _aimRange, _firstAimingPoint) && Physics.Raycast(_headView.position, _headView.forward, _aimRange, _secondAimingPoint);
    }

    private void TrackPoition()
    {
        _headView.position = new Vector3(_tempAimingPoint.position.x, _aimingPoint.position.y, _aimingPoint.position.z);
    }
}
