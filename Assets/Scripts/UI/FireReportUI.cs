using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FireReportUI : MonoBehaviour
{
    [Header("Fire Report UI")]
    [SerializeField] private GameObject _successFulFireReportPanel;
    [SerializeField] private GameObject _failedFireReportPanel;
    [SerializeField] private Image _aircraftImage;
    [SerializeField] private RectTransform _hitPointRect;
    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _angleSettingsText;
    [SerializeField] private TextMeshProUGUI _launchModeText;
    [SerializeField] private TextMeshProUGUI _differenceBetweenLockAndFireText;

    [Header("Aircraft Sprites")]
    [SerializeField] private Sprite _mi24Sprite;
    [SerializeField] private Sprite _su25Sprite;

    [Header("References")]
    [SerializeField] private Strela2MHUD _instructorPanel;
    [SerializeField] private Strela2MLauncher _strela2MLauncher;

    private List<FireResultData> _fireResultDatas = new List<FireResultData>();

    private int _resultDataIndex = 0;

    private FireResultData _currentFireResultData;

    private void OnEnable()
    {
        AviationManager.SimulationEnded += DisplayFireResultData;
        AircraftCollisionManager.DamagedReceived += AddFireResultData;
        Strela2MInput.TriggerPullingEnded += DetectCurrentTarget;
    }

    private void OnDisable()
    {
        AviationManager.SimulationEnded -= DisplayFireResultData;
        AircraftCollisionManager.DamagedReceived -= AddFireResultData;
        Strela2MInput.TriggerPullingEnded -= DetectCurrentTarget;
    }

    public void OnNextButtonClick()
    {
        DisplayFireResultData();

        _resultDataIndex++;

        if (_resultDataIndex == _fireResultDatas.Count)
        {
            _resultDataIndex = 0;
        }
    }

    private void DisplayFireResultData()
    {
        if (_fireResultDatas.Count == 0)
        {
            _failedFireReportPanel.SetActive(true);

            return;
        }

        _successFulFireReportPanel.SetActive(true);

        _hitPointRect.anchoredPosition = _fireResultDatas[_resultDataIndex].HitPoint;
        _targetNameText.text = $"სამიზნე ობიექტი: {_fireResultDatas[_resultDataIndex].TargetObjectName}";
        _angleSettingsText.text = $"გადახრა: {_fireResultDatas[_resultDataIndex].AngleSettings}";
        _launchModeText.text = $"სროლის რეჟიმი: {_fireResultDatas[_resultDataIndex].LaunchMode}";
        _differenceBetweenLockAndFireText.text = $"სამიზნის ჩაჭერიდან გასროლის დრო: {_fireResultDatas[_resultDataIndex].DifferenceBetweeenLockAndFire:F1}";
        _aircraftImage.sprite = _fireResultDatas[_resultDataIndex].TargetSprite;
    }

    private void AddFireResultData(IAircraftTarget target, bool isDamageCritical)
    {
        _fireResultDatas.Add(_currentFireResultData);
    }

    private void DetectCurrentTarget()
    {
        Transform currentTarget = _strela2MLauncher.CurrentSeeker.CurrentTarget;

        if (currentTarget != null)
        {
            IAircraftTarget target = currentTarget.GetComponent<IAircraftTarget>();

            if (target != null)
            {
                AircraftType aircraftType = target.GetAircraftType();

                _currentFireResultData = new FireResultData()
                {
                    TargetObjectName = aircraftType.ToString(),
                    TargetSprite = aircraftType == AircraftType.MI24 ? _mi24Sprite : _su25Sprite,
                    AngleSettings = Mathf.Abs(_instructorPanel.AngleSettings),
                    LaunchMode = _instructorPanel.MissileLaunchMode,
                    DifferenceBetweeenLockAndFire = Time.time - _strela2MLauncher.CurrentSeeker.TargetLockTime,
                    HitPoint = aircraftType == AircraftType.MI24 ? new Vector3(220f, 80f, 0f) : new Vector3(0f, -20f, 0f)
                };

                Debug.Log(_currentFireResultData.TargetObjectName);
            }
        }
    }
}
