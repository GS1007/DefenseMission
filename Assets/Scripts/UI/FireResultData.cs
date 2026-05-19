using UnityEngine;

public class FireResultData
{
    public string TargetObjectName { get; set; }
    public AircraftType TypeOfAircraft { get; set; }
    public float AngleSettings { get; set; }
    public string LaunchMode { get; set; }
    public Vector3 HitPoint { get; set; }
    public Sprite TargetSprite { get; set; }
}
