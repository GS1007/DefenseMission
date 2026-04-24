using UnityEngine.Splines;

public class AviationSequentialMovementPathStrategy : IAviationMovementPathStrategy
{
    private readonly SplineContainer[] _movementPaths;

    private int _currentPathIndex = 0;

    public AviationSequentialMovementPathStrategy(SplineContainer[] movementPaths)
    {
        _movementPaths = movementPaths;
    }

    public SplineContainer GetMovementPath()
    {
        if (_currentPathIndex >= _movementPaths.Length)
        {
            return null;
        }

        SplineContainer path = _movementPaths[_currentPathIndex];

        _currentPathIndex++;

        return path;
    }
}
