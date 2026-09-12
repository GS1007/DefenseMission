using System;
using TMPro;
using UnityEngine;

public class SimulationSetupPanel : MonoBehaviour
{
    public event Action<int> AddAircraftRequested;
    public event Action LaunchRequested;

    [SerializeField] private Canvas _simulationSetupCanvas;

    [SerializeField] private TMP_Dropdown _aircraftSelectionDropDown;

    public void OnAddButtonClick()
    {
        AddAircraftRequested?.Invoke(_aircraftSelectionDropDown.value);
    }

    public void OnLaunchButtonClick()
    {
        _simulationSetupCanvas.enabled = false;
        LaunchRequested?.Invoke();
    }
}
