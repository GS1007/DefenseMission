using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedAircraftDisplay : MonoBehaviour
{
    public event Action EditButtonClicked;
    public event Action RemoveButtonClicked;

    [SerializeField] private Image _aircraftImage;
    [SerializeField] private TextMeshProUGUI _aircraftNameText;

    public void Init(AircraftConfig aircraftConfig)
    {
        _aircraftImage.sprite = aircraftConfig.AircraftSprite;
        _aircraftNameText.text = aircraftConfig.AircraftName;
    }

    public void OnEdit_ButtonClick()
    {
        EditButtonClicked?.Invoke();
    }

    public void OnRemove_ButtonClick()
    {
        RemoveButtonClicked?.Invoke();
    }
}
