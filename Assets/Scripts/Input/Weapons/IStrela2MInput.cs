
using UnityEngine.InputSystem;

public interface IStrela2MInput
{
    public void OnPowerToggle(InputAction.CallbackContext context);
    public void OnTriggerPullingStart(InputAction.CallbackContext context);
    public void OnTriggerPullingEnd(InputAction.CallbackContext context);
}
