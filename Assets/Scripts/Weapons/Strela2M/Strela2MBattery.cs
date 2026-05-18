using System;
using System.Collections;
using UnityEngine;

public class Strela2MBattery : MonoBehaviour
{
    public static event Action PowerUpStarted;
    public static event Action PoweredOn;
    public static event Action BatteryDied;

    [SerializeField] private Strela2MLauncher _launcher;

    [SerializeField] private float _maxBatteryLife = 0f;
    [SerializeField] private float _gyroSpinupTime = 0f;

    private float _currentBatteryTime = 0f;

    private void OnEnable()
    {
        Strela2MInput.PowerToggled += PowerUp;
    }

    private void Update()
    {
        if (_launcher.State == LauncherState.SpinningUp || _launcher.State == LauncherState.Ready)
        {
            _currentBatteryTime -= Time.deltaTime;

            if (_currentBatteryTime <= 0)
            {
                KillBattery();
            }
        }
    }

    private void OnDisable()
    {
        Strela2MInput.PowerToggled -= PowerUp;
    }

    private void PowerUp()
    {
        if (_launcher.State == LauncherState.Off)
        {
            _currentBatteryTime = _maxBatteryLife;

            StartCoroutine(SpinupRoutine());
        }
    }

    private void KillBattery()
    {
        BatteryDied?.Invoke();
        Debug.Log("BATTERY DEAD. The weapon is disabled.");
    }

    private IEnumerator SpinupRoutine()
    {
        PowerUpStarted?.Invoke();

        yield return new WaitForSeconds(_gyroSpinupTime);

        PoweredOn?.Invoke();

        Debug.Log("Powered on");
    }
}
