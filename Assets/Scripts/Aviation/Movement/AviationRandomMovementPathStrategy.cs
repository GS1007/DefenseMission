using UnityEngine.Splines;

public class AviationRandomMovementPathStrategy : IAviationMovementPathStrategy
{
    private readonly SplineContainer[] _movementPaths;

    public AviationRandomMovementPathStrategy(SplineContainer[] movementPaths)
    {
        _movementPaths = movementPaths;
    }

    public SplineContainer GetMovementPath()
    {
        return _movementPaths[UnityEngine.Random.Range(0, _movementPaths.Length)];
    }
}
