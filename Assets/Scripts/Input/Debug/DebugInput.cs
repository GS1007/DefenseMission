using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInput : MonoBehaviour, IDebugInput
{
    public event Action AngleSetupStickToggled;
    public event Action FireReportOpened;

    [SerializeField] private InputActionReference _angleSetupStickToggleActionReference;
    [SerializeField] private InputActionReference _fireReportOpenInputActionReference;

    private void OnEnable()
    {
        _angleSetupStickToggleActionReference.action.Enable();
        _fireReportOpenInputActionReference.action.Enable();

        _angleSetupStickToggleActionReference.action.performed += OnAngleSetupStickToggle;
        _fireReportOpenInputActionReference.action.performed += OnFireReportOpen;
    }

    private void OnDisable()
    {
        _angleSetupStickToggleActionReference.action.performed -= OnAngleSetupStickToggle;
        _fireReportOpenInputActionReference.action.performed -= OnFireReportOpen;

        _angleSetupStickToggleActionReference.action.Disable();
        _fireReportOpenInputActionReference.action.Disable();
    }

    private void OnAngleSetupStickToggle(InputAction.CallbackContext context)
    {
        AngleSetupStickToggled?.Invoke();
    }

    private void OnFireReportOpen(InputAction.CallbackContext context)
    {
        FireReportOpened?.Invoke();
    }
}
