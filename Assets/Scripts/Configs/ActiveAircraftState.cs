using UnityEngine;
using UnityEngine.Splines;

public class ActiveAircraftState : MonoBehaviour
{
    public SplineContainer AircraftPath { get; set; }
    public AircraftConfig AircraftConfig { get; }

    public float Speed { get; set; }
    public float Height { get; set; }

    public ActiveAircraftState(AircraftConfig aircraftConfig)
    {
        AircraftConfig = aircraftConfig;

        if (AircraftConfig != null)
        {
            Speed = AircraftConfig.BaseFlightSpeed;
            Height = AircraftConfig.BaseFlightHeight;
        }
    }
}
