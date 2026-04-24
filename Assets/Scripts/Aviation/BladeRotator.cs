using UnityEngine;

public class BladeRotator : MonoBehaviour
{
    [SerializeField] private Transform _blade;

    [SerializeField] private float _rotationSpeed = 0f;

    private void Update()
    {
        _blade.Rotate(_rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
