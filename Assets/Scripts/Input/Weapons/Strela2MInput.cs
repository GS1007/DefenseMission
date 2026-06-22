using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strela2MInput : MonoBehaviour, IStrela2MInput
{
    public static event Action PowerToggled;
    public static event Action TriggerPullingStarted;
    public static event Action TriggerPullingEnded;
    public static event Action LauncherReseted;
    public static event Action AngleSetupStickToggled;
    public static event Action TrackingReseted;
    public static event Action FireReportOpened;

    [SerializeField] private InputActionReference _powerToggleReference;
    [SerializeField] private InputActionReference _triggerPullingStartActionReference;
    [SerializeField] private InputActionReference _triggerPullingEndActionReference;
    [SerializeField] private InputActionReference _launcherResetActionReference;
    [SerializeField] private InputActionReference _angleSetupStickToggleActionReference;
    [SerializeField] private InputActionReference _trackingResetInputActionReference;
    [SerializeField] private InputActionReference _fireReportOpenInputActionReference;

    private void OnEnable()
    {
        _powerToggleReference.action.Enable();
        _triggerPullingStartActionReference.action.Enable();
        _triggerPullingEndActionReference.action.Enable();
        _launcherResetActionReference.action.Enable();
        _angleSetupStickToggleActionReference.action.Enable();
        _trackingResetInputActionReference.action.Enable();
        _fireReportOpenInputActionReference.action.Enable();

        _powerToggleReference.action.performed += OnPowerToggle;
        _triggerPullingStartActionReference.action.performed += OnTriggerPullingStart;
        _triggerPullingEndActionReference.action.performed += OnTriggerPullingEnd;
        _launcherResetActionReference.action.performed += OnLauncherReset;
        _angleSetupStickToggleActionReference.action.performed += OnAngleSetupStickToggle;
        _trackingResetInputActionReference.action.performed += OnTrackingReset;
        _fireReportOpenInputActionReference.action.performed += OnFireReportOpen;
    }

    private void OnDisable()
    {
        _triggerPullingEndActionReference.action.performed -= OnTriggerPullingEnd;
        _triggerPullingStartActionReference.action.performed -= OnTriggerPullingStart;
        _powerToggleReference.action.performed -= OnPowerToggle;
        _launcherResetActionReference.action.performed -= OnLauncherReset;
        _angleSetupStickToggleActionReference.action.performed -= OnAngleSetupStickToggle;
        _trackingResetInputActionReference.action.performed -= OnTrackingReset;
        _fireReportOpenInputActionReference.action.performed -= OnFireReportOpen;

        _triggerPullingEndActionReference.action.Disable();
        _triggerPullingStartActionReference.action.Disable();
        _powerToggleReference.action.Disable();
        _launcherResetActionReference.action.Disable();
        _angleSetupStickToggleActionReference.action.Disable();
        _trackingResetInputActionReference.action.Disable();
        _fireReportOpenInputActionReference.action.Disable();
    }

    public void OnPowerToggle(InputAction.CallbackContext context)
    {
        PowerToggled?.Invoke();
    }

    public void OnTriggerPullingStart(InputAction.CallbackContext context)
    {
        TriggerPullingStarted?.Invoke();
    }

    public void OnTriggerPullingEnd(InputAction.CallbackContext context)
    {
        TriggerPullingEnded?.Invoke();
    }

    public void OnLauncherReset(InputAction.CallbackContext context)
    {
        LauncherReseted?.Invoke();
    }
    public void OnAngleSetupStickToggle(InputAction.CallbackContext context)
    {
        AngleSetupStickToggled?.Invoke();
    }

    public void OnTrackingReset(InputAction.CallbackContext context)
    {
        TrackingReseted?.Invoke();
    }

    public void OnFireReportOpen(InputAction.CallbackContext context)
    {
        FireReportOpened?.Invoke();
    }
}
