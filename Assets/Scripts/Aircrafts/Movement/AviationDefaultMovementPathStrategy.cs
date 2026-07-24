using UnityEngine;
using UnityEngine.Splines;

public class AviationDefaultMovementPathStrategy : IAviationMovementPathStrategy
{
    private readonly SplineContainer _movementPath;

    public AviationDefaultMovementPathStrategy(SplineContainer movementPath)
    {
        _movementPath = movementPath;
    }

    public SplineContainer GetMovementPath()
    {
        return _movementPath;
    }
}
