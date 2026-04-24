using System;
using System.Collections;
using UnityEngine;

public class Strela2MBattery : MonoBehaviour
{
    public event Action BatteryEnabled;

    [SerializeField] private float _batteryLifeSeconds = 0f;
    [SerializeField] private float _startupDelay = 0f;

    private bool _isPoweredOn = false;
    private bool _isStartingUp = false;

    private float _currentBatteryLife;

    public bool IsPoweredOn { get { return _isPoweredOn; } }

    private void OnEnable()
    {
        Strela2MInput.PowerToggled += TogglePower;

        Strela2MEvents.BatteryDied += ResetPower;
        Strela2MEvents.Fired += ResetPower;

    }

    private void Start()
    {
        _currentBatteryLife = _batteryLifeSeconds;
    }

    private void Update()
    {
        if (_isPoweredOn == true)
        {
            HandleBatteryLife();
        }
    }

    private void OnDisable()
    {
        Strela2MEvents.Fired -= ResetPower;
        Strela2MEvents.BatteryDied -= ResetPower;

        Strela2MInput.PowerToggled -= TogglePower;
    }

    private void TogglePower()
    {
        if (_isPoweredOn || _isStartingUp || _currentBatteryLife <= 0) return;

        StartCoroutine(PowerUpSequence());
    }

    private void HandleBatteryLife()
    {
        _currentBatteryLife -= Time.deltaTime;

        Strela2MEvents.TriggerBatteryHealthUpdateEvent(Mathf.RoundToInt(_currentBatteryLife / _batteryLifeSeconds * 100));

        if (_currentBatteryLife <= 0)
        {
            Strela2MEvents.TriggerBatteryDeathEvent();

            Debug.Log("Battery is dead!");
        }
    }

    private void ResetPower()
    {
        _isPoweredOn = false;
        _currentBatteryLife = _batteryLifeSeconds;
    }

    private IEnumerator PowerUpSequence()
    {
        _isStartingUp = true;

        yield return new WaitForSeconds(_startupDelay);

        Debug.Log("Powered On");

        _isStartingUp = false;
        _isPoweredOn = true;

        BatteryEnabled?.Invoke();
    }
}
