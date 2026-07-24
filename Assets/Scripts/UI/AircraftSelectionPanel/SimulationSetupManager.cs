using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SimulationSetupManager : MonoBehaviour
{
    [Header("UI Panels & Containers")]
    [SerializeField] private SimulationSetupPanel _simulationSetupPanel;
    [SerializeField] private SelectedAircraftEditPanel _editPanel;
    [SerializeField] private RectTransform _aircraftListContentContainer;
    
    [Header("Prefabs & Systems")]
    [SerializeField] private AircraftListEntryUI _aircraftListEntryPrefab;
    [SerializeField] private SplineContainer _splineContainerPrefab;
    [SerializeField] private AviationManager _aviationManager;

    [Header("Data")]
    [SerializeField] private AircraftConfig[] _aircraftConfigs;

    private List<ActiveAircraftState> _activeStates = new List<ActiveAircraftState>();
    private Dictionary<AircraftListEntryUI, ActiveAircraftState> _uiToDataMap = new Dictionary<AircraftListEntryUI, ActiveAircraftState>();
    
    private AircraftListEntryUI _activeEditingDisplay;

    private void OnEnable()
    {
        _simulationSetupPanel.AddAircraftRequested += OnAddAircraft;
        _simulationSetupPanel.LaunchRequested += OnLaunch;
        
        _editPanel.DataChanged += OnCurrentAircraftDataChanged;
        _editPanel.BackButtonClicked += CloseEditPanel;
    }

    private void OnDisable()
    {
        _simulationSetupPanel.AddAircraftRequested -= OnAddAircraft;
        _simulationSetupPanel.LaunchRequested -= OnLaunch;
        
        _editPanel.DataChanged -= OnCurrentAircraftDataChanged;
        _editPanel.BackButtonClicked -= CloseEditPanel;
    }

    private void Start()
    {
        CloseEditPanel();
    }

    private void OnAddAircraft(int dropDownIndex)
    {
        if (dropDownIndex >= 0 && dropDownIndex < _aircraftConfigs.Length)
        {
            AircraftConfig config = _aircraftConfigs[dropDownIndex];

            ActiveAircraftState activeState = new ActiveAircraftState(config);
            
            SplineContainer newPath = Instantiate(_splineContainerPrefab);
            newPath.transform.position = new Vector3(newPath.transform.position.x, activeState.Height, newPath.transform.position.z);
            activeState.AircraftPath = newPath;

            AircraftListEntryUI listEntryUI = Instantiate(_aircraftListEntryPrefab, _aircraftListContentContainer, false);
            listEntryUI.Init(config);
            
            listEntryUI.EditButtonClicked += OnOpenEditWindow;
            listEntryUI.RemoveButtonClicked += OnRemoveAircraft;

            _activeStates.Add(activeState);
            _uiToDataMap[listEntryUI] = activeState;
        }
    }

    private void OnOpenEditWindow(AircraftListEntryUI listEntryUI)
    {
        if (_uiToDataMap.TryGetValue(listEntryUI, out ActiveAircraftState activeState))
        {
            _activeEditingDisplay = listEntryUI;
            _editPanel.gameObject.SetActive(true);
            _editPanel.BindTarget(activeState);
        }
    }

    private void OnCurrentAircraftDataChanged()
    {
        if (_activeEditingDisplay != null && _uiToDataMap.TryGetValue(_activeEditingDisplay, out ActiveAircraftState activeState))
        {
            _activeEditingDisplay.RefreshDisplay(activeState.Speed, activeState.Height);
        }
    }

    private void CloseEditPanel()
    {
        _activeEditingDisplay = null;
        _editPanel.gameObject.SetActive(false);
    }

    private void OnRemoveAircraft(AircraftListEntryUI listEntryUI)
    {
        if (_uiToDataMap.TryGetValue(listEntryUI, out ActiveAircraftState activeState))
        {
            listEntryUI.EditButtonClicked -= OnOpenEditWindow;
            listEntryUI.RemoveButtonClicked -= OnRemoveAircraft;

            if (_activeEditingDisplay == listEntryUI)
            {
                CloseEditPanel();
            }

            _activeStates.Remove(activeState);
            _uiToDataMap.Remove(listEntryUI);
            
            Destroy(activeState.AircraftPath.gameObject);
            Destroy(listEntryUI.gameObject);
        }
    }

    private void OnLaunch()
    {
        foreach (ActiveAircraftState activeState in _activeStates)
        {
            _aviationManager.SetupAircraft(activeState);
        }

        _aviationManager.LaunchAircraft();
    }
}
