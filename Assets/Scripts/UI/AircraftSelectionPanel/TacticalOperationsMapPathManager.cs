using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TacticalOperationsMapPathManager : MonoBehaviour
{
    private const float MAP_WORLD_SIZE_HALF = 4000f;

    [Header("Prefabs & References")]
    [SerializeField] private GameObject _waypointPrefab;
    [SerializeField] private GameObject _linePrefab;

    [SerializeField] private Canvas _tacticalOperationsMapCanvas;
    [SerializeField] private RectTransform _tacticalOperationsMapRectTransform;

    [SerializeField] private TacticalOperationsMapUI _tacticalOperationsMapUI;

    private readonly List<TacticalOperationsMapWaypoint> _waypoints = new List<TacticalOperationsMapWaypoint>();
    private readonly List<RectTransform> _lineSegments = new List<RectTransform>();

    [Header("Line Settings")]
    [SerializeField] private float _lineThickness = 6f;
    [SerializeField][Range(5, 20)] private int _uiCurveResolution = 10;

    private ActiveAircraftState _currentAircraftState;

    private void OnEnable()
    {
        _tacticalOperationsMapUI.OnDoubleClick += AddWaypointAtScreenPosition;
    }

    private void OnDisable()
    {
        _tacticalOperationsMapUI.OnDoubleClick -= AddWaypointAtScreenPosition;
    }

    public void BindAircraft(ActiveAircraftState aircraftState)
    {
        _currentAircraftState = aircraftState;

        foreach (var wp in _waypoints)
        {
            wp.WaypointRightButtonClicked -= OnWaypointRightButtonClick;
            Destroy(wp.gameObject);
        }
        _waypoints.Clear();

        foreach (var line in _lineSegments)
        {
            Destroy(line.gameObject);
        }
        _lineSegments.Clear();

        if (_currentAircraftState != null && _currentAircraftState.AircraftPath != null)
        {
            Spline spline = _currentAircraftState.AircraftPath.Spline;
            foreach (var knot in spline)
            {
                Vector2 uiPos = LocalSplineToUIPosition(knot.Position);
                CreateWaypointUI(uiPos);
            }

            UpdatePath();
        }
    }

    public void AddWaypoint()
    {
        Vector2 localSpawnPos = Vector2.zero;

        if (_waypoints.Count > 0)
        {
            RectTransform lastWp = _waypoints[_waypoints.Count - 1].GetComponent<RectTransform>();
            localSpawnPos = lastWp.anchoredPosition + new Vector2(60f, -60f);
        }

        CreateWaypointUI(localSpawnPos);
        UpdatePath();
    }

    public void AddWaypointAtScreenPosition(Vector2 screenPosition)
    {
        Camera cam = _tacticalOperationsMapCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _tacticalOperationsMapCanvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_tacticalOperationsMapRectTransform, screenPosition, cam, out Vector2 localPoint);

        CreateWaypointUI(localPoint);
        UpdatePath();
    }

    private void CreateWaypointUI(Vector2 localPosition)
    {
        GameObject wpObj = Instantiate(_waypointPrefab, _tacticalOperationsMapRectTransform);
        RectTransform wpRect = wpObj.GetComponent<RectTransform>();
        wpRect.anchoredPosition = localPosition;

        TacticalOperationsMapWaypoint wpScript = wpObj.GetComponent<TacticalOperationsMapWaypoint>();
        wpScript.Init(this, _tacticalOperationsMapCanvas, _tacticalOperationsMapRectTransform);
        _waypoints.Add(wpScript);
        wpScript.WaypointRightButtonClicked += OnWaypointRightButtonClick;
    }

    public void UpdatePath()
    {
        SyncToSpline();

        if (_currentAircraftState == null || _currentAircraftState.AircraftPath == null || _waypoints.Count < 2)
        {
            foreach (var lineSegment in _lineSegments) lineSegment.gameObject.SetActive(false);
            return;
        }

        Spline spline = _currentAircraftState.AircraftPath.Spline;

        int requiredLines = (spline.Count - 1) * _uiCurveResolution;

        while (_lineSegments.Count < requiredLines)
        {
            GameObject lineObj = Instantiate(_linePrefab, _tacticalOperationsMapRectTransform);
            lineObj.transform.SetAsFirstSibling();
            _lineSegments.Add(lineObj.GetComponent<RectTransform>());
        }

        for (int i = 0; i < _lineSegments.Count; i++)
        {
            _lineSegments[i].gameObject.SetActive(i < requiredLines);
        }

        float step = 1f / requiredLines;

        for (int i = 0; i < requiredLines; i++)
        {
            float tStart = i * step;
            float tEnd = (i + 1) * step;

            Vector3 pos3DStart = (Vector3)spline.EvaluatePosition(tStart);
            Vector3 pos3DEnd = (Vector3)spline.EvaluatePosition(tEnd);

            Vector2 uiStart = LocalSplineToUIPosition(pos3DStart);
            Vector2 uiEnd = LocalSplineToUIPosition(pos3DEnd);

            RectTransform line = _lineSegments[i];
            Vector2 delta = uiEnd - uiStart;

            line.anchoredPosition = uiStart + delta * 0.5f;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            line.localEulerAngles = new Vector3(0, 0, angle);
            line.sizeDelta = new Vector2(delta.magnitude, _lineThickness);
        }
    }

    private void SyncToSpline()
    {
        if (_currentAircraftState == null || _currentAircraftState.AircraftPath == null) return;

        Spline spline = _currentAircraftState.AircraftPath.Spline;

        while (spline.Count < _waypoints.Count) spline.Add(new BezierKnot());
        while (spline.Count > _waypoints.Count) spline.RemoveAt(spline.Count - 1);

        for (int i = 0; i < _waypoints.Count; i++)
        {
            RectTransform wpRect = _waypoints[i].GetComponent<RectTransform>();
            Vector3 localSplinePos = UIToLocalSplinePosition(wpRect.anchoredPosition);

            BezierKnot knot = spline[i];
            knot.Position = localSplinePos;
            spline[i] = knot;

            spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
    }

    public void OnWaypointRightButtonClick(TacticalOperationsMapWaypoint waypoint)
    {
        if (_waypoints.Contains(waypoint) == true)
        {
            waypoint.WaypointRightButtonClicked -= OnWaypointRightButtonClick;
            _waypoints.Remove(waypoint);
            Destroy(waypoint.gameObject);

            UpdatePath();
        }
    }

    private Vector3 UIToLocalSplinePosition(Vector2 uiPos)
    {
        float mapHalfWidth = _tacticalOperationsMapRectTransform.rect.width * 0.5f;
        float mapHalfHeight = _tacticalOperationsMapRectTransform.rect.height * 0.5f;

        float localX = (uiPos.x / mapHalfWidth) * MAP_WORLD_SIZE_HALF;
        float localZ = (uiPos.y / mapHalfHeight) * MAP_WORLD_SIZE_HALF;

        return new Vector3(localX, 0f, localZ);
    }

    private Vector2 LocalSplineToUIPosition(Vector3 localPos)
    {
        float mapHalfWidth = _tacticalOperationsMapRectTransform.rect.width * 0.5f;
        float mapHalfHeight = _tacticalOperationsMapRectTransform.rect.height * 0.5f;

        float uiX = (localPos.x / MAP_WORLD_SIZE_HALF) * mapHalfWidth;
        float uiY = (localPos.z / MAP_WORLD_SIZE_HALF) * mapHalfHeight;

        return new Vector2(uiX, uiY);
    }
}