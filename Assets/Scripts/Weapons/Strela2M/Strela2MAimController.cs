using UnityEngine;

public class Strela2MAimController : MonoBehaviour
{
    [SerializeField] private Transform _strela;
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Transform _headPoint;

    [SerializeField] private WeaponTransformDataSO _weaponTransformDataVRHolding;
    [SerializeField] private WeaponTransformDataSO _weaponTransformDataVRAiming;

    [SerializeField] private float _aimDistance = 0f;
    [SerializeField] private float _aimAngle = 0f;

    [SerializeField] private Vector3 _snapPointOffset = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AimingPoint") == true)
        {
            AttachWeaponToCamera();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AimingPoint") == true)
        {
            AttachWeaponToController();
        }
    }

    private void AttachWeaponToCamera()
    {
        _strela.parent = _headPoint;
        _strela.localPosition = _weaponTransformDataVRAiming.Position;
        _strela.localRotation = Quaternion.Euler(_weaponTransformDataVRAiming.Rotation);
    }

    private void AttachWeaponToController()
    {
        _strela.parent = _weaponHolder;
        _strela.localPosition = _weaponTransformDataVRHolding.Position;
        _strela.localRotation = Quaternion.Euler(_weaponTransformDataVRHolding.Rotation);
    }

    //private void CheckAngle()
    //{
    //    float distance = Vector3.Distance(_vrCamera.position, _firstAimPoint.position);
    //    Vector3 dirToWeapon = (_firstAimPoint.position - _vrCamera.position).normalized;
    //    float angle = Vector3.Angle(_vrCamera.forward, dirToWeapon);

    //    if(distance < _aimDistance && angle < _aimAngle)
    //    {

    //    }
    //    else
    //    {

    //    }
    //}
}
