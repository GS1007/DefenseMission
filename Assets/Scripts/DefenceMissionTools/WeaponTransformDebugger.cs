using TMPro;
using UnityEngine;

public class WeaponTransformDebugger : MonoBehaviour
{
    [SerializeField] private Transform _weaponTreansform;

    [SerializeField] private TextMeshProUGUI _positionText;
    [SerializeField] private TextMeshProUGUI _rotationText;

    private void OnEnable()
    {
        Strela2MEvents.Fired += DisplayWeaponTransform;
    }

    private void OnDisable()
    {
        Strela2MEvents.Fired -= DisplayWeaponTransform;
    }

    private void DisplayWeaponTransform()
    {
        _positionText.text = $"Pos X:{_weaponTreansform.localPosition.x} Y:{_weaponTreansform.localPosition.y} Z:{_weaponTreansform.localPosition.z}";

        float rotX = Mathf.DeltaAngle(0, _weaponTreansform.eulerAngles.x);
        float rotY = Mathf.DeltaAngle(0, _weaponTreansform.eulerAngles.y);
        float rotZ = Mathf.DeltaAngle(0, _weaponTreansform.eulerAngles.z);

        _rotationText.text = $"Rot X:{rotX:F1} Y:{rotY:F1} Z:{rotZ:F1}";
    }
}
