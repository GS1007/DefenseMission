using UnityEngine;

public class OcclusionObjectAngleConfig : MonoBehaviour
{
    [SerializeField] private float _detectionAngle = 0;

    public float DetectionAngle { get { return _detectionAngle; } }
}
