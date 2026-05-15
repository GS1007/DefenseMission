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

    public string CurrentTargetName { get; private set; }
    public string MissileLaunchMode { get; private set; }
    public Vector3 AngleSettings { get { return _strela2M.eulerAngles; } }
    public AircraftType TypeOfAircraft { get { return _seeker.CurrentTarget.GetComponent<IAircraftTarget>().GetAircraftType(); } }

    private void OnEnable()
    {
        Strela2MLauncher.IllegallyFired += EnableNotificationtext;
        Strela2MLauncher.MissileLoaded += OnMissileLoad;
        Strela2MLauncher.LaunchModeSet += DisplayLaunchMode;
    }

    private void Update()
    {
        if (_seeker.HasLock == true)
        {
            DisplayAngleSetupValues();

            if (_seeker.CurrentTarget != null && _seeker.CurrentTarget.name.Equals(CurrentTargetName) == false)
            {
                DisplayCurrentTargetName(_seeker.CurrentTarget.name);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(CurrentTargetName) == false)
            {
                CurrentTargetName = string.Empty;
            }
        }
    }

    private void OnDisable()
    {
        Strela2MLauncher.IllegallyFired -= EnableNotificationtext;
        Strela2MLauncher.MissileLoaded -= OnMissileLoad;
        Strela2MLauncher.LaunchModeSet -= DisplayLaunchMode;
    }

    private void OnMissileLoad(Strela2MMissile missile)
    {
        _seeker = missile.Seeker;
    }

    private void DisplayCurrentTargetName(string name)
    {
        _targetNameText.text = $"სამიზნე ობიექტი: {name}";
        CurrentTargetName = name;

        Debug.Log(CurrentTargetName);
    }

    private void DisplayAngleSetupValues()
    {
        _angleSetupText.text = $"გადახრები Y:{_strela2M.eulerAngles.y}";
    }

    private void DisplayLaunchMode(LaunchMode launchMode)
    {
        MissileLaunchMode = launchMode == LaunchMode.Automatic ? "აუტომატური" : "მექანიკური";
        _launchModeText.text = $"სროლის რეჟიმი: {MissileLaunchMode}";
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
