using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _instructorPanel;
    [SerializeField] private GameObject _resultPanel;

    private void OnEnable()
    {
        AviationManager.SimulationEnded += OnSimulationEnd;
    }

    private void OnDisable()
    {
        AviationManager.SimulationEnded -= OnSimulationEnd;
    }

    public void RelaodGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnSimulationEnd()
    {
        _instructorPanel.SetActive(false);
        _resultPanel.SetActive(true);
    }
}
