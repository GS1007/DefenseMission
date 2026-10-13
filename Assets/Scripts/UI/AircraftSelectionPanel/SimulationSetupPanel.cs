using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSetupPanel : MonoBehaviour
{
    public event Action<int> AddAircraftRequested;
    public event Action LaunchRequested;

    [SerializeField] private Canvas _simulationSetupCanvas;

    [SerializeField] private TMP_Dropdown _aircraftSelectionDropDown;

    [SerializeField] private Button _launchButton;

    public void InitData(AircraftConfig[] aircraftConfigs)
    {
        List<TMP_Dropdown.OptionData> optionsList = new List<TMP_Dropdown.OptionData>();

        foreach (var config in aircraftConfigs)
        {
            optionsList.Add(new TMP_Dropdown.OptionData()
            {
                text = config.AircraftName
            });
        }

        _aircraftSelectionDropDown.AddOptions(optionsList);
    }

    public void OnAddButtonClick()
    {
        AddAircraftRequested?.Invoke(_aircraftSelectionDropDown.value);
    }

    public void SetlaunchButtonState(int aircraftsCount)
    {
        if(_launchButton != null)
        {
            _launchButton.interactable = aircraftsCount > 0;
        }
    }

    public void OnLaunchButtonClick()
    {
        _simulationSetupCanvas.enabled = false;
        LaunchRequested?.Invoke();
    }
}
