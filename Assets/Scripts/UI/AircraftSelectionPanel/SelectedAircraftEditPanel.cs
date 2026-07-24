using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAircraftEditPanel : MonoBehaviour
{
    public event Action BackButtonClicked;
    public event Action DataChanged;

    [Header("UI Controls")]
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private Slider _heightSlider;

    [Header("UI Text Overlays")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _speedValueText;
    [SerializeField] private TextMeshProUGUI _heightValueText;
    [SerializeField] private TextMeshProUGUI _totalAircraftText;

    private ActiveAircraftState _currentTarget;

    public void BindTarget(ActiveAircraftState target)
    {
        _currentTarget = target;
        AircraftConfig config = target.AircraftConfig;

        if (_titleText != null) _titleText.text = $"{config.AircraftName} - რედაქტირება";

        _speedSlider.minValue = config.MinFlightSpeed;
        _speedSlider.maxValue = config.MaxFlightSpeed;
        _heightSlider.minValue = config.MinFlightHeight;
        _heightSlider.maxValue = config.MaxFlightHeight;

        _speedSlider.value = _currentTarget.Speed;
        _heightSlider.value = _currentTarget.Height;

        UpdateTextDisplays();
    }

    public void OnFlightAltitudeValueChange(float newValue)
    {
        if (_currentTarget == null) return;
        
        _currentTarget.Height = newValue;
        
        if (_currentTarget.AircraftPath != null)
        {
            Vector3 currentPos = _currentTarget.AircraftPath.transform.position;
            _currentTarget.AircraftPath.transform.position = new Vector3(currentPos.x, newValue, currentPos.z);
        }

        UpdateTextDisplays();
        DataChanged?.Invoke();
    }

    public void OnAircraftSpeedValueChange(float newValue)
    {
        if (_currentTarget == null) return;
        
        _currentTarget.Speed = newValue;
        
        UpdateTextDisplays();
        DataChanged?.Invoke();
    }

    public void OnBackButtonClick()
    {
        BackButtonClicked?.Invoke();
    }

    public void UpdateTotalAircraftCount(int totalCount)
    {
        if (_totalAircraftText != null)
        {
            _totalAircraftText.text = $"საფრენი აპარატები: {totalCount}"; 
        }
    }

    private void UpdateTextDisplays()
    {
        if (_speedValueText != null) _speedValueText.text = $"სიჩქარე: {Mathf.RoundToInt(_currentTarget.Speed)}მ/წ";
        if (_heightValueText != null) _heightValueText.text = $"სიმაღლე: {Mathf.RoundToInt(_currentTarget.Height)}მ";
    }
}
