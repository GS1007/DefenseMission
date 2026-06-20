using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class GameViewRenderController : MonoBehaviour
{
    [SerializeField] private InputActionReference _leftEyeGameViewInputActionReference;
    [SerializeField] private InputActionReference _rightEyeGameViewInputActionReference;
    [SerializeField] private InputActionReference _bothEyesGameViewInputActionReference;

    private void OnEnable()
    {
        _leftEyeGameViewInputActionReference.action.Enable();
        _rightEyeGameViewInputActionReference.action.Enable();
        _bothEyesGameViewInputActionReference.action.Enable();

        _leftEyeGameViewInputActionReference.action.performed += SetGameViewToLeftEye;
        _rightEyeGameViewInputActionReference.action.performed += SetGameViewToRightEye;
        _bothEyesGameViewInputActionReference.action.performed += SetGameViewToBothEyes;
    }

    private void OnDisable()
    {
        _leftEyeGameViewInputActionReference.action.performed -= SetGameViewToLeftEye;
        _rightEyeGameViewInputActionReference.action.performed -= SetGameViewToRightEye;
        _bothEyesGameViewInputActionReference.action.performed -= SetGameViewToBothEyes;

        _leftEyeGameViewInputActionReference.action.Disable();
        _rightEyeGameViewInputActionReference.action.Disable();
        _bothEyesGameViewInputActionReference.action.Disable();
    }

    private void SetGameViewToLeftEye(InputAction.CallbackContext context)
    {
        XRSettings.gameViewRenderMode = GameViewRenderMode.LeftEye;
    }

    private void SetGameViewToRightEye(InputAction.CallbackContext context)
    {
        XRSettings.gameViewRenderMode = GameViewRenderMode.RightEye;
    }

    private void SetGameViewToBothEyes(InputAction.CallbackContext context)
    {
        XRSettings.gameViewRenderMode = GameViewRenderMode.BothEyes;
    }
}
