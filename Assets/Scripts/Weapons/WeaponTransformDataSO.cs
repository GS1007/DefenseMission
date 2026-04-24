using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponsTransformData", menuName = "Weapons Transform Data")]
public class WeaponTransformDataSO : ScriptableObject
{
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Vector3 _scale;

    public Vector3 Position { get { return _position; } }
    public Vector3 Rotation { get { return _rotation; } }
    public Vector3 Scale { get { return _scale; } }
}
