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

    [Header("Aircraft Sprites")]
    [SerializeField] private Sprite _mi24Sprite;
    [SerializeField] private Sprite _su25Sprite;

    [Header("References")]
    [SerializeField] private Strela2MHUD _instructorPanel;

    private List<FireResultData> _fireResultDatas = new List<FireResultData>();

    private int _resultDataIndex = 0;

    private void OnEnable()
    {
        Strela2MLauncher.MissileLaunched += AddFireResultData;
        AviationManager.SimulationEnded += DisplayFireResultData;
    }

    private void OnDisable()
    {
        Strela2MLauncher.MissileLaunched -= AddFireResultData;
        AviationManager.SimulationEnded -= DisplayFireResultData;
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
        _angleSettingsText.text = $"გადახრა: {_fireResultDatas[_resultDataIndex].AngleSettings.ToString()}";
        _launchModeText.text = $"სროლის რეჟიმი {_fireResultDatas[_resultDataIndex].LaunchMode}";
        _aircraftImage.sprite = _fireResultDatas[_resultDataIndex].TargetSprite;
    }

    private void AddFireResultData(bool hit)
    {
        _fireResultDatas.Add(new FireResultData()
        {
            TargetObjectName = _instructorPanel.CurrentTargetName,
            TargetSprite = _instructorPanel.TypeOfAircraft == AircraftType.MI24 ? _mi24Sprite : _su25Sprite,
            TypeOfAircraft = _instructorPanel.TypeOfAircraft,
            AngleSettings = _instructorPanel.AngleSettings,
            LaunchMode = _instructorPanel.MissileLaunchMode,
            HitPoint = _instructorPanel.TypeOfAircraft == AircraftType.MI24 ?
            (hit ? new Vector3(Random.Range(0f, 50f), Random.Range(-25f, 25f), 0f) : new Vector3(Random.Range(-90f, -25f), 25f, 0f)) : (hit ? new Vector3(Random.Range(0f, 50f), Random.Range(-25f, 25f), 0f) : new Vector3(Random.Range(-90f, -25f), 25f, 0f))
        });
    }
}
