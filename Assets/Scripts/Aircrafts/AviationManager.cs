using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AviationManager : MonoBehaviour
{
    public static event Action SimulationEnded;

    [SerializeField] private int _aircraftLaunchingMinDelay = 0;
    [SerializeField] private int _aircraftLaunchingMaxDelay = 0;

    private List<AviationSplinePathMovement> _splinePathMovements;

    private int _currentAirCraftIndex = 0;

    public void SetupAircraft(ActiveAircraftState activeState)
    {
        GameObject aircraftInstance = Instantiate(activeState.AircraftConfig.AircraftPrefab);
        AviationSetup aircraftSetup = aircraftInstance.GetComponent<AviationSetup>();

        if(aircraftSetup != null)
        {
            aircraftSetup.Setup(activeState.AircraftPath);
        }

        AviationSplinePathMovement aircraftSplinePathMovement = aircraftInstance.GetComponent<AviationSplinePathMovement>();

        if (aircraftSplinePathMovement != null)
        {
            _splinePathMovements.Add(aircraftSplinePathMovement);
        }
    }

    public void LaunchAircraft()
    {
        if (_currentAirCraftIndex >= _splinePathMovements.Count)
        {
            Debug.Log("Simulation is over");

            SimulationEnded?.Invoke();

            return;
        }

        StartCoroutine(StartLaunchingAircraft());
    }

    private IEnumerator StartLaunchingAircraft()
    {
        int delay = UnityEngine.Random.Range(_aircraftLaunchingMinDelay, _aircraftLaunchingMaxDelay);

        yield return new WaitForSeconds(delay);

        _splinePathMovements[_currentAirCraftIndex].gameObject.SetActive(true);
        _splinePathMovements[_currentAirCraftIndex].Move();
        _currentAirCraftIndex++;
    }
}
