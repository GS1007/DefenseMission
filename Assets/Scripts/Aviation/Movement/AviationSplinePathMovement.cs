using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class AviationSplinePathMovement : MonoBehaviour, IAviationSplinePathMovement
{
    [SerializeField] private SplineAnimate _splineAnimate;

    [SerializeField] private UnityEvent _movementPathCompleted;

    private IAviationMovementPathStrategy _movementPathStrategy;

    private void OnEnable()
    {
        _splineAnimate.Completed += OnMovementPathComplete;
    }

    private void OnDisable()
    {
        _splineAnimate.Completed -= OnMovementPathComplete;
    }

    public void SetupMovementPathStrategy(IAviationMovementPathStrategy movementPathStrategy)
    {
        _movementPathStrategy = movementPathStrategy;
    }

    public void Move()
    {
        _splineAnimate.Container = _movementPathStrategy.GetMovementPath();

        _splineAnimate.Play();
    }

    private void OnMovementPathComplete()
    {
        _movementPathCompleted?.Invoke();
    }
}
