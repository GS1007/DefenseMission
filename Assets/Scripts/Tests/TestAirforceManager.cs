using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class TestAirforceManager : MonoBehaviour
{
    [SerializeField] private AviationSplinePathMovement[] _aviationSplinePathMovements;

    [SerializeField] private SplineContainer[] _splineContainers;

    [SerializeField] private FireReportUI _fireReporttUI;

    [SerializeField] private int _aircraftLaunchingMinDelay = 3;
    [SerializeField] private int _aircraftLaunchingMaxDelay = 5;

    [Header("UI")]
    [SerializeField] private GameObject _instructorPanel;
    [SerializeField] private GameObject _resultPanel;
    
    private int _currentAirCraftIndex = 0;

    private void Start()
    {
        LaunchAircraft();
    }

    public void LaunchAircraft()
    {
        if (_currentAirCraftIndex >= _aviationSplinePathMovements.Length)
        {
            Debug.Log("Simulation is over");

            OnSimulationEnd();
            _fireReporttUI.DisplayFireResultData();

            return;
        }

        SetupAircraft();
        StartCoroutine(StartLaunchingAircraft());
    }

    private void SetupAircraft()
    {
        AviationSetup aviationSetup = _aviationSplinePathMovements[_currentAirCraftIndex].GetComponent<AviationSetup>();
        aviationSetup.Setup(_splineContainers[_currentAirCraftIndex]);
    }

    private void OnSimulationEnd()
    {
        _instructorPanel.SetActive(false);
        _resultPanel.SetActive(true);
    }

    private IEnumerator StartLaunchingAircraft()
    {
        int delay = Random.Range(_aircraftLaunchingMinDelay, _aircraftLaunchingMaxDelay);

        yield return new WaitForSeconds(delay);

        _aviationSplinePathMovements[_currentAirCraftIndex].gameObject.SetActive(true);
        _aviationSplinePathMovements[_currentAirCraftIndex].Move();
        _currentAirCraftIndex++;
    }
}
