using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Strela2MHUD : MonoBehaviour
{
    [SerializeField] private Image _progressImage;
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Strela2MLauncher _launcher;
    [SerializeField] private Strela2MBattery _battery;

    private Strela2MSeeker _seeker;

    private void OnEnable()
    {
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
    }

    private void Update()
    {
        if (_launcher.State != LauncherState.Ready || _launcher.LoadedMissile == null)
        {
            return;
        }

        DisplayBatteryHealth();
        DisplayLockProgress();
    }

    private void OnDisable()
    {
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
    }

    public void DisplayStatus(string statusMessage)
    {
        _statusText.text = statusMessage;
    }

    private void DisplayBatteryHealth()
    {
        _statusText.text = $"BATTERY: {_battery.BatteryHealth}%";
    }

    private void DisplayLockProgress()
    {
        _progressImage.fillAmount = _seeker.LockProgress;
    }

    private void OnMissileLoad(Strela2MMissile missile)
    {
        _seeker = missile.Seeker;
    }
}
