using System;
using TMPro;
using UnityEngine;

public class SimulationSetupPanel : MonoBehaviour
{
    public event Action<int> AddAircraftRequested;
    public event Action LaunchRequested;

    [SerializeField] private TMP_Dropdown _aircraftSelectionDropDown;

    public void OnAdd_ButtonClick()
    {
        AddAircraftRequested?.Invoke(_aircraftSelectionDropDown.value);
    }

    public void OnLaunch_ButtonClick()
    {
        LaunchRequested?.Invoke();
    }
}
