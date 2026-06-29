using UnityEngine;

[CreateAssetMenu(fileName = "New Aircraft Config", menuName = "Configs/Aircraft")]
public class AircraftConfig : ScriptableObject
{
    [SerializeField] private string _aircraftName;
    [SerializeField] private Sprite _aircraftSprite;

    public string AircraftName { get { return _aircraftName; } }
    public Sprite AircraftSprite { get { return _aircraftSprite; } }
}
