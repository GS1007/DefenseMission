using UnityEngine;
using UnityEngine.Splines;

public class AviationSetup : MonoBehaviour
{
    [SerializeField] private AviationSplinePathMovement _movementController;

    public void Setup(SplineContainer path)
    {
        _movementController.SetupMovementPathStrategy(new AviationDefaultMovementPathStrategy(path));
    }
}