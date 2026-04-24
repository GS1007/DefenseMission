using System;
using System.Collections;
using UnityEngine;

public class AviationManager : MonoBehaviour
{
    public static event Action SimulationEnded;

    [SerializeField] private AviationSplinePathMovement[] _splinePathMovements;

    [SerializeField] private int _aircraftLaunchingMinDelay = 0;
    [SerializeField] private int _aircraftLaunchingMaxDelay = 0;

    private int _currentAirCraftIndex = 0;

    private void Start()
    {
        LaunchAircraft();
    }

    public void LaunchAircraft()
    {
        if (_currentAirCraftIndex >= _splinePathMovements.Length)
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
