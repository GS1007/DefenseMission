using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAircraftEditPanel : MonoBehaviour
{
    public event Action BackButtonClicked;
    public event Action DataChanged;
    public event Action<int> AircraftOptionChanged;

    [Header("UI Controls")]
    [SerializeField] private Slider _heightSlider;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private TMP_Dropdown _aircraftTypeDropdown;

    [Header("UI Text Overlays")]
    [SerializeField] private TextMeshProUGUI _currentHeightValueText;
    [SerializeField] private TextMeshProUGUI _minimumHeightValueText;
    [SerializeField] private TextMeshProUGUI _maximumHeightValueText;
    [SerializeField] private TextMeshProUGUI _currentSpeedValueText;
    [SerializeField] private TextMeshProUGUI _minimumSpeedValueText;
    [SerializeField] private TextMeshProUGUI _maximumSpeedValueText;
    [SerializeField] private TextMeshProUGUI _totalAircraftText;

    private ActiveAircraftState _currentTarget;

    public void BindTarget(ActiveAircraftState target)
    {
        _currentTarget = target;
        AircraftConfig config = target.AircraftConfig;

        _speedSlider.minValue = config.MinFlightSpeed;
        _speedSlider.maxValue = config.MaxFlightSpeed;
        _heightSlider.minValue = config.MinFlightHeight;
        _heightSlider.maxValue = config.MaxFlightHeight;

        _speedSlider.value = _currentTarget.Speed;
        _heightSlider.value = _currentTarget.Height;

        _heightSlider.onValueChanged.AddListener(OnHeightValueChange); 
        _speedSlider.onValueChanged.AddListener(OnSpeedValueChange);

        DisplayDefaultStats();
        UpdateTextDisplays();
    }

    public void OnBackButtonClick()
    {
        _heightSlider.onValueChanged.RemoveListener(OnHeightValueChange);
        _speedSlider.onValueChanged.RemoveListener(OnSpeedValueChange);
        BackButtonClicked?.Invoke();
    }

    public void UpdateTotalAircraftCount(int totalCount)
    {
        if (_totalAircraftText != null)
        {
            _totalAircraftText.text = $"საფრენი აპარატები: {totalCount}"; 
        }
    }

    public void SetupAircraftOptions(List<ActiveAircraftState> aircaftStates)
    {
        _aircraftTypeDropdown.options = new List<TMP_Dropdown.OptionData>(capacity: aircaftStates.Count);

        for(int i = 0; i < aircaftStates.Count; ++i)
        {
            _aircraftTypeDropdown.options.Add(new TMP_Dropdown.OptionData()
            {
                text = $"{i + 1}. {aircaftStates[i].AircraftConfig.AircraftName}",
            });
        }
    }

    public void DisplaySelectedAircraft(int value)
    {
        _aircraftTypeDropdown.value = value;
    }

    public void OnAircraftDropdownOptionChange()
    {
        AircraftOptionChanged?.Invoke(_aircraftTypeDropdown.value);
    }

    private void OnHeightValueChange(float newValue)
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

    private void OnSpeedValueChange(float newValue)
    {
        if (_currentTarget == null) return;

        _currentTarget.Speed = newValue;

        UpdateTextDisplays();
        DataChanged?.Invoke();
    }

    private void UpdateTextDisplays()
    {
        if (_currentSpeedValueText != null) _currentSpeedValueText.text = $"სიჩქარე: {Mathf.RoundToInt(_currentTarget.Speed)}მ/წ";
        if (_currentHeightValueText != null) _currentHeightValueText.text = $"სიმაღლე: {Mathf.RoundToInt(_currentTarget.Height)}მ";
    }

    private void DisplayDefaultStats()
    {
        if (_minimumHeightValueText != null) _minimumHeightValueText.text = $"{_currentTarget.AircraftConfig.MinFlightHeight}მ";
        if (_maximumHeightValueText != null) _maximumHeightValueText.text = $"{_currentTarget.AircraftConfig.MaxFlightHeight}მ";
        if (_minimumSpeedValueText != null) _minimumSpeedValueText.text = $"{_currentTarget.AircraftConfig.MinFlightSpeed}მ";
        if (_maximumSpeedValueText != null) _maximumSpeedValueText.text = $"{_currentTarget.AircraftConfig.MaxFlightSpeed}მ";
    }
}
