using UnityEngine;

public class FireResultData
{
    public string TargetObjectName { get; set; }
    public float AngleSettings { get; set; }
    public string LaunchMode { get; set; }
    public float DifferenceBetweeenLockAndFire { get; set; }
    public Vector3 HitPoint { get; set; }
    public Sprite TargetSprite { get; set; }
}
