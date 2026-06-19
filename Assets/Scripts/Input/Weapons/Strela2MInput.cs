using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strela2MInput : MonoBehaviour, IStrela2MInput
{
    public static event Action PowerToggled;
    public static event Action TriggerPullingStarted;
    public static event Action TriggerPullingEnded;
    public static event Action LauncherReseted;

    [SerializeField] private InputActionReference _powerToggleReference;
    [SerializeField] private InputActionReference _triggerPullingStartActionReference;
    [SerializeField] private InputActionReference _triggerPullingEndActionReference;
    [SerializeField] private InputActionReference _launcherResetActionReference;

    private void OnEnable()
    {
        _powerToggleReference.action.Enable();
        _triggerPullingStartActionReference.action.Enable();
        _triggerPullingEndActionReference.action.Enable();
        _launcherResetActionReference.action.Enable();

        _powerToggleReference.action.performed += OnPowerToggle;
        _triggerPullingStartActionReference.action.performed += OnTriggerPullingStart;
        _triggerPullingEndActionReference.action.performed += OnTriggerPullingEnd;
        _launcherResetActionReference.action.performed += OnLauncherReset;
    }

    private void OnDisable()
    {
        _triggerPullingEndActionReference.action.performed -= OnTriggerPullingEnd;
        _triggerPullingStartActionReference.action.performed -= OnTriggerPullingStart;
        _powerToggleReference.action.performed -= OnPowerToggle;
        _launcherResetActionReference.action.performed -= OnLauncherReset;

        _triggerPullingEndActionReference.action.Disable();
        _triggerPullingStartActionReference.action.Disable();
        _powerToggleReference.action.Disable();
        _launcherResetActionReference.action.Disable();
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
}
