using UnityEngine;

public class Strela2MLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer _light;
    [SerializeField] private Strela2MLauncher _launcher;

    private Strela2MSeeker _seeker;

    private void OnEnable()
    {
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
        Strela2MBattery.BatteryDied += DisableLight;
        Strela2MLauncher.Fired += OnFire;
    }

    private void OnDisable()
    {
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MBattery.BatteryDied -= DisableLight;
        Strela2MLauncher.Fired -= OnFire;
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
        else if (_seeker.CurrentTargetType == TargetType.Sun || (_seeker.SignalStrength > 0f && _seeker.SignalStrength < 0.6f))
        {
            _light.enabled = (Time.time % 0.4f) > 0.2f;
        }
        else
        {
            _light.enabled = false;
        }
    }

    private void OnMissileLoad(Strela2MMissile missile)
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