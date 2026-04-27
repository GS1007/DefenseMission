using UnityEngine;

public class Strela2MAimingPoint : MonoBehaviour
{
    [SerializeField] private Transform _headView;
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Transform _strela;

    [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRHolding;
    [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRAiming;


    private void OnTriggerEnter(Collider other)
    {
        //_strela.parent = _headView;
        //_strela.localPosition = _objectTransformSettingsVRAiming.Position;
        //_strela.localRotation = Quaternion.Euler(_objectTransformSettingsVRAiming.Rotation);
        Debug.Log("Should Switch Point");
    }

    private void OnTriggerExit(Collider other)
    {
        //_strela.parent = _weaponHolder;
        //_strela.localPosition = _objectTransformSettingsVRHolding.Position;
        //_strela.localRotation = Quaternion.Euler(_objectTransformSettingsVRHolding.Rotation);
        //Debug.Log("Should Not Switch Point");
    }
}
