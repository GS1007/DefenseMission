using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strela2MInput : MonoBehaviour, IManpadsInput
{
    public event Action PowerToggled;
    public event Action TriggerPullingStarted;
    public event Action TriggerPullingEnded;
    public event Action LauncherReseted;
    public event Action TrackingReseted;

    [SerializeField] private InputActionReference _powerToggleReference;
    [SerializeField] private InputActionReference _triggerPullingStartActionReference;
    [SerializeField] private InputActionReference _triggerPullingEndActionReference;
    [SerializeField] private InputActionReference _launcherResetActionReference;
    [SerializeField] private InputActionReference _trackingResetInputActionReference;

    private void OnEnable()
    {
        _powerToggleReference.action.Enable();
        _triggerPullingStartActionReference.action.Enable();
        _triggerPullingEndActionReference.action.Enable();
        _launcherResetActionReference.action.Enable();
        _trackingResetInputActionReference.action.Enable();

        _powerToggleReference.action.performed += OnPowerToggle;
        _triggerPullingStartActionReference.action.performed += OnTriggerPullingStart;
        _triggerPullingEndActionReference.action.performed += OnTriggerPullingEnd;
        _launcherResetActionReference.action.performed += OnLauncherReset;
        _trackingResetInputActionReference.action.performed += OnTrackingReset;
    }

    private void OnDisable()
    {
        _triggerPullingEndActionReference.action.performed -= OnTriggerPullingEnd;
        _triggerPullingStartActionReference.action.performed -= OnTriggerPullingStart;
        _powerToggleReference.action.performed -= OnPowerToggle;
        _launcherResetActionReference.action.performed -= OnLauncherReset;
        _trackingResetInputActionReference.action.performed -= OnTrackingReset;

        _triggerPullingEndActionReference.action.Disable();
        _triggerPullingStartActionReference.action.Disable();
        _powerToggleReference.action.Disable();
        _launcherResetActionReference.action.Disable();
        _trackingResetInputActionReference.action.Disable();
    }

    private void OnPowerToggle(InputAction.CallbackContext context)
    {
        PowerToggled?.Invoke();
    }

    private void OnTriggerPullingStart(InputAction.CallbackContext context)
    {
        TriggerPullingStarted?.Invoke();
    }

    private void OnTriggerPullingEnd(InputAction.CallbackContext context)
    {
        TriggerPullingEnded?.Invoke();
    }

    private void OnLauncherReset(InputAction.CallbackContext context)
    {
        LauncherReseted?.Invoke();
    }

    private void OnTrackingReset(InputAction.CallbackContext context)
    {
        TrackingReseted?.Invoke();
    }
}
