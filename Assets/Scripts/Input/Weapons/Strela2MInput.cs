using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strela2MInput : MonoBehaviour, IStrela2MInput
{
    public static event Action PowerToggled;
    public static event Action TriggerPullingStarted;
    public static event Action TriggerPullingEnded;


    [SerializeField] private InputActionReference _powerToggleReference;
    [SerializeField] private InputActionReference _triggerPullingStartActionReference;
    [SerializeField] private InputActionReference _triggerPullingEndActionReference;


    private void OnEnable()
    {
        _powerToggleReference.action.Enable();
        _triggerPullingStartActionReference.action.Enable();
        _triggerPullingEndActionReference.action.Enable();

        _powerToggleReference.action.performed += OnPowerToggle;
        _triggerPullingStartActionReference.action.performed += OnTriggerPullingStart;
        _triggerPullingEndActionReference.action.performed += OnTriggerPullingEnd;
    }

    private void OnDisable()
    {
        _triggerPullingEndActionReference.action.performed -= OnTriggerPullingEnd;
        _triggerPullingStartActionReference.action.performed -= OnTriggerPullingStart;
        _powerToggleReference.action.performed -= OnPowerToggle;

        _triggerPullingEndActionReference.action.Disable();
        _triggerPullingStartActionReference.action.Disable();
        _powerToggleReference.action.Disable();
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
}
