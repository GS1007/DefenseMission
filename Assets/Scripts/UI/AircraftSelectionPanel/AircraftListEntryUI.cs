using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AircraftListEntryUI : MonoBehaviour
{
    public event Action<AircraftListEntryUI> EditButtonClicked;
    public event Action<AircraftListEntryUI> RemoveButtonClicked;

    [SerializeField] private Image _aircraftImage;
    [SerializeField] private TextMeshProUGUI _aircraftNameText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _heightText;

    public void Init(AircraftConfig aircraftConfig)
    {
        _aircraftImage.sprite = aircraftConfig.AircraftSprite;
        _aircraftNameText.text = aircraftConfig.AircraftName;
        RefreshDisplay(aircraftConfig.BaseFlightSpeed, aircraftConfig.BaseFlightHeight);
    }

    public void RefreshDisplay(float speed, float height)
    {
        _speedText.text = $"სიჩქარე: {Mathf.RoundToInt(speed)}მ/წ";
        _heightText.text = $"სიმაღლე: {Mathf.RoundToInt(height)}მ";
    }

    public void OnEdit_ButtonClick()
    {
        EditButtonClicked?.Invoke(this);
    }

    public void OnRemove_ButtonClick()
    {
        RemoveButtonClicked?.Invoke(this);
    }
}
