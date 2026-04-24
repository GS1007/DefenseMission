using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Strela2MHUD : MonoBehaviour
{
    [SerializeField] private Image _progressRing;
    [SerializeField] private TextMeshProUGUI _statusText;

    private void OnEnable()
    {
        Strela2MEvents.BatteryHealthUpdated += OnBatteryHealthUpdate;
    }

    private void OnDisable()
    {
        Strela2MEvents.BatteryHealthUpdated -= OnBatteryHealthUpdate;
    }

    public void DisplayStatus(string statusMessage)
    {
        _statusText.text = statusMessage;
    }

    private void OnBatteryHealthUpdate(int batteryHealth)
    {
        _statusText.text = $"BATTERY: {batteryHealth}%";
    }
}
