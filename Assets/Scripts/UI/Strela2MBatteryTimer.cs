using TMPro;
using UnityEngine;

public class Strela2MBatteryTimer : MonoBehaviour
{
    private const float TIMER_UPDATE_FREQUENCY = 1.0f;

    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private Strela2MBattery _battery;

    private bool _timerRunning = false;

    private float _timeLeft;
    private float _nextTimeToTimerUpdate;

    private void OnEnable()
    {
        Strela2MBattery.PowerUpStarted += LaunchTimer;
        Strela2MBattery.BatteryDied += StopTimer;
    }

    private void Start()
    {
        _timerText.text = _battery.MaxBatteryLife.ToString();
    }

    private void Update()
    {
        if(_timerRunning)
        {
            if(Time.time >= _nextTimeToTimerUpdate)
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
    }

    private void LaunchTimer()
    {
        _timerRunning = true;
        _timeLeft = _battery.MaxBatteryLife;
        _nextTimeToTimerUpdate = Time.time + TIMER_UPDATE_FREQUENCY;
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        _timeLeft--;
        _timerText.text = _timeLeft.ToString();
    }

    private void StopTimer()
    {
        _timerRunning = false;
        _timeLeft = 0f;
        _nextTimeToTimerUpdate = 0f;
    }
}
