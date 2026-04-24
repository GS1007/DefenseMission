using UnityEngine;
using UnityEngine.Splines;

public class AviationSetup : MonoBehaviour
{
    [SerializeField] private AviationSplinePathMovement _movementController;

    [SerializeField] private SplineContainer[] _availablePaths;

    private void Awake()
    {
        _movementController.SetupMovementPathStrategy(new AviationRandomMovementPathStrategy(_availablePaths));
    }
}