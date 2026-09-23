using UnityEngine;

public class Strela2MLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer _light;
    [SerializeField] private ManpadsLauncher _launcher;

    private ISeeker _seeker;

    private void OnEnable()
    {
        ManpadsLauncher.MissileLoaded += OnMissileLoad;
        Strela2MBattery.BatteryDied += DisableLight;
        ManpadsLauncher.Fired += OnFire;
    }

    private void OnDisable()
    {
        ManpadsLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MBattery.BatteryDied -= DisableLight;
        ManpadsLauncher.Fired -= OnFire;
    }

    private void Update()
    {
        if (_launcher.State != LauncherState.Ready || _launcher.LoadedMissile == null || _seeker == null)
        {
            _light.enabled = false;
            return;
        }

        UpdateLight();
    }

    private void UpdateLight()
    {
        if (_seeker.HasLock == true)
        {
            _light.enabled = true;
        }
        else if (_seeker.CurrentTargetType == TargetType.Sun || _seeker.CurrentTargetType == TargetType.Cloud || (_seeker.SignalStrength > 0f && _seeker.SignalStrength < 0.6f))
        {
            _light.enabled = (Time.time % 0.4f) > 0.2f;
        }
        else
        {
            _light.enabled = false;
        }
    }

    private void OnMissileLoad(GuidedMissile missile)
    {
        _seeker = missile.Seeker;
    }

    private void OnFire()
    {
        _seeker = null;
        DisableLight();
    }

    private void DisableLight()
    {
        _light.enabled = false;
    }
}