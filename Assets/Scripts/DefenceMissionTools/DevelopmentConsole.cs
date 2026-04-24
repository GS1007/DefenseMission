
using UnityEngine;

namespace DefenceMissionTools
{
    public class DevelopmentConsole : MonoBehaviour
    {
        private enum Environment { VR_Holding, VR_AIMING, PC }

        [SerializeField] private Transform _gameObjectTransform;

        [SerializeField] private Environment _environment = Environment.VR_Holding;

        [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRHolding;
        [SerializeField] private WeaponTransformDataSO _objectTransformSettingsVRAiming;
        [SerializeField] private WeaponTransformDataSO _objectTransformSettingsPC;

        private void OnValidate()
        {
            SetupGameObjectTransform();
        }

        private void SetupGameObjectTransform()
        {
            WeaponTransformDataSO gameObjectTransformSettings;

            switch (_environment)
            {
                case Environment.PC:
                    {
                        gameObjectTransformSettings = _objectTransformSettingsPC;
                        break;
                    }
                case Environment.VR_Holding:
                    {
                        gameObjectTransformSettings = _objectTransformSettingsVRHolding;
                        break;
                    }
                case Environment.VR_AIMING:
                    {
                        gameObjectTransformSettings = _objectTransformSettingsVRAiming;
                        break;
                    }
                default:
                    {
                        gameObjectTransformSettings = _objectTransformSettingsPC;
                        break;
                    }
            }

            if (_gameObjectTransform.parent == null)
            {
                _gameObjectTransform.position = gameObjectTransformSettings.Position;
                _gameObjectTransform.rotation = Quaternion.Euler(gameObjectTransformSettings.Rotation);
            }
            else
            {
                _gameObjectTransform.localPosition = gameObjectTransformSettings.Position;
                _gameObjectTransform.localRotation = Quaternion.Euler(gameObjectTransformSettings.Rotation);
            }
        }
    }
}