using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private float _gameRestartDelay = 0f;

    private void OnEnable()
    {
        AviationManager.SimulationEnded += OnSimulationEnd;
    }

    private void OnDisable()
    {
        AviationManager.SimulationEnded -= OnSimulationEnd;
    }

    private void OnSimulationEnd()
    {
        StartCoroutine(RestartSimulation());
    }

    private IEnumerator RestartSimulation()
    {
        yield return new WaitForSeconds(_gameRestartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
