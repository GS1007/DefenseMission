using TMPro;
using UnityEngine;

public class Strela2MHUD : MonoBehaviour
{
    [SerializeField] private Transform _strela2M;

    [SerializeField] private TextMeshProUGUI _targetNameText;
    [SerializeField] private TextMeshProUGUI _angleSetupText;
    [SerializeField] private TextMeshProUGUI _launchModeText;
    [SerializeField] private TextMeshProUGUI _notificationText;

    [SerializeField] private Strela2MLauncher _launcher;

    [SerializeField] private float _notificationDissapearTime = 3f;

    private Strela2MSeeker _seeker;

    private string _currentTargetName;

    private void OnEnable()
    {
        Strela2MLauncher.IllegallyFired += EnableNotificationtext;
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
        Strela2MLauncher.Fired += DisplayLaunchMode;
    }

    private void Update()
    {
        if (_seeker.HasLock == true)
        {
            DisplayAngleSetupValues();

            if (_seeker.CurrentTarget.name.Equals(_currentTargetName) == false)
            {
                DisplayCurrentTargetName(_seeker.CurrentTarget.name);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(_currentTargetName) == false)
            {
                _currentTargetName = string.Empty;
            }
        }
    }

    private void OnDisable()
    {
        Strela2MLauncher.IllegallyFired -= EnableNotificationtext;
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MLauncher.Fired -= DisplayLaunchMode;
    }

    private void OnMissileLoad(Strela2MMissile missile)
    {
        _seeker = missile.Seeker;
    }

    private void DisplayCurrentTargetName(string name)
    {
        _targetNameText.text = $"სამიზნე ობიექტი: {name}";
        _currentTargetName = name;
    }

    private void DisplayAngleSetupValues()
    {
        _angleSetupText.text = $"გადახრები Y:{_strela2M.eulerAngles.y}";
    }

    private void DisplayLaunchMode()
    {
        _launchModeText.text = $"სროლის რეჟიმი: {_launcher.MissileLaunchMode.ToString()}";
    }

    private void EnableNotificationtext()
    {
        _notificationText.enabled = true;

        Invoke(nameof(DisableNotificationtext), _notificationDissapearTime);
    }

    private void DisableNotificationtext()
    {
        _notificationText.enabled = false;
    }
}
