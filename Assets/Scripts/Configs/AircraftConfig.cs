using UnityEngine;

[CreateAssetMenu(fileName = "New Aircraft Config", menuName = "Configs/Aircraft")]
public class AircraftConfig : ScriptableObject
{
    [SerializeField] private string _aircraftName;
    [SerializeField] private float _baseFlightSpeed;
    [SerializeField] private float _baseFlightHeight;
    [SerializeField] private Sprite _aircraftSprite;
    [SerializeField] private GameObject _aircraftPrefab;

    [Header("Simulation Constraints")]
    [SerializeField] private float _minFlightSpeed = 30f;
    [SerializeField] private float _maxFlightSpeed = 600f;
    [SerializeField] private float _minFlightHeight = 50f;
    [SerializeField] private float _maxFlightHeight = 10000f;

    public string AircraftName => _aircraftName;
    public float BaseFlightSpeed => _baseFlightSpeed;
    public float BaseFlightHeight => _baseFlightHeight;
    public Sprite AircraftSprite => _aircraftSprite;
    public GameObject AircraftPrefab => _aircraftPrefab;

    public float MinFlightSpeed => _minFlightSpeed;
    public float MaxFlightSpeed => _maxFlightSpeed;
    public float MinFlightHeight => _minFlightHeight;
    public float MaxFlightHeight => _maxFlightHeight;
}
