using TMPro;
using UnityEngine;

public class Strela2MBatteryTimer : MonoBehaviour
{
    private const float TIMER_UPDATE_FREQUENCY = 1.0f;

    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private Strela2MBattery _battery;

    private bool _timerRunning = false;

    private float _timeElapsed;
    private float _nextTimeToTimerUpdate;

    private void OnEnable()
    {
        Strela2MBattery.PowerUpStarted += LaunchTimer;
        Strela2MBattery.BatteryDied += StopTimer;
        Strela2MLauncher.Fired += StopTimer;
    }

    private void Start()
    {
        _timeElapsed = 0f;
        _timerText.text = $"0{_timeElapsed}";
    }

    private void Update()
    {
        if (_timerRunning)
        {
            if (Time.time >= _nextTimeToTimerUpdate)
            {
                UpdateTimer();
                _nextTimeToTimerUpdate = Time.time + TIMER_UPDATE_FREQUENCY;
            }
        }
    }

    private void OnDisable()
    {
        Strela2MBattery.PowerUpStarted -= LaunchTimer;
        Strela2MBattery.BatteryDied -= StopTimer;
        Strela2MLauncher.Fired -= StopTimer;
    }

    private void LaunchTimer()
    {
        _timerRunning = true;
        _nextTimeToTimerUpdate = Time.time + TIMER_UPDATE_FREQUENCY;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        ++_timeElapsed;
        _timerText.text = _timeElapsed < 10 ? $"0{_timeElapsed}" : _timeElapsed.ToString();
    }

    private void StopTimer()
    {
        _timerRunning = false;
        _timeElapsed = 0f;
        _nextTimeToTimerUpdate = 0f;
    }
}
