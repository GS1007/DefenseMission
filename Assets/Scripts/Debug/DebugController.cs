using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _debugInputBehaviour;
    [SerializeField] private GameObject _angleSetupStick;
    [SerializeField] private GameObject _reportPanel;

    [SerializeField] private FireReportUI _fireReportUI;

    private IDebugInput _debugInput;

    private void Awake()
    {
        _debugInput = _debugInputBehaviour as IDebugInput;
    }

    private void OnEnable()
    {
        if(_debugInput != null)
        {
            _debugInput.AngleSetupStickToggled += OnAngleSetupStickToggle;
            _debugInput.FireReportOpened += OnFireReportOpen_ButtonClick;
        }
    }

    private void OnDisable()
    {
        if (_debugInput != null)
        {
            _debugInput.AngleSetupStickToggled -= OnAngleSetupStickToggle;
            _debugInput.FireReportOpened -= OnFireReportOpen_ButtonClick;
        }
    }

    private void OnAngleSetupStickToggle()
    {
        _angleSetupStick.SetActive(!_angleSetupStick.activeSelf);
    }

    private void OnFireReportOpen_ButtonClick()
    {
        if (_reportPanel.activeSelf == false)
        {
            _reportPanel.SetActive(true);
            _fireReportUI.DisplayFireResultData();
        }
        else
        {
            _reportPanel.SetActive(false);
        }
    }
}
