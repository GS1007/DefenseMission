using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public class AviationMovementPathsContainer
{
    [SerializeField] private SplineContainer[] _availablePaths;

    public SplineContainer[] AvailablePaths { get { return _availablePaths; } }
}
