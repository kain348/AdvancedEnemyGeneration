using System.Collections.Generic;
using UnityEngine;

public enum PathTraversalMode
{
    SinglePass,
    Loop,
    PingPong
}

[AddComponentMenu("Game/Movement/Target Waypoint Mover")]
public class TargetWaypointMover : MonoBehaviour
{
    [Header("Path Settings")]
    [Tooltip("List of points of the path in the world space.")]
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    [Tooltip("Speed of movement along the path.")]
    [SerializeField] private float _speed = 2f;

    [Tooltip("The distance at which we think the point has been reached.")]
    [SerializeField] private float _reachDistance = 0.1f;

    [Header("Loop Settings")]
    [Tooltip("Path mode: one time, round or back and forth.")]
    [SerializeField] private PathTraversalMode _mode = PathTraversalMode.Loop;

    [Tooltip("Start motion automatically when the scene starts.")]
    [SerializeField] private bool _playOnStart = true;

    private const int ForwardDirection = 1;
    private const int BackwardDirection = -1;

    private int _currentIndex;
    private int _direction = ForwardDirection;
    private bool _isMoving;

    private void Start()
    {
        if(HasValidPath() == false)
        {
            Debug.LogWarning($"{nameof(TargetWaypointMover)} on the {name} object does not have a character path (minimum of 1 point).", this);
            enabled = false;

            return;
        }

        SnapToFirstWaypoint();

        if (_playOnStart)
            _isMoving = true;
    }

    private void Update()
    {
        if (CanMove() == false)
            return;

        Transform targetPoint = GetCurrentWaypoint();
        Vector3 toPoint = targetPoint.position - transform.position;
        float sqrDistance = toPoint.sqrMagnitude;

        if (HasReachedWaypoint(sqrDistance))
        {
            AdvanceToNextWaypoint();

            return;
        }

        MoveTowards(toPoint);
    }

    private void OnDrawGizmos()
    {
        if (HasValidPath() == false)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < _waypoints.Count; i++)
        {
            Transform waypoint = _waypoints[i];
            if (waypoint == null) 
                continue;

            Gizmos.DrawSphere(waypoint.position, 0.1f);

            if (i < _waypoints.Count - 1 && _waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoint.position, _waypoints[i + 1].position);
            }

            if (_mode == PathTraversalMode.Loop && i == _waypoints.Count - 1 && _waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoint.position, _waypoints[0].position);
            }
        }
    }

    public void Play()
    {
        if (HasValidPath() == false)
            return;

        _isMoving = true;
    }

    public void Stop()
    {
        _isMoving = false;
    }

    private bool HasValidPath()
    {
        return _waypoints != null && _waypoints.Count > 0;
    }

    private bool CanMove()
    {
        return _isMoving && HasValidPath();
    }

    private void SnapToFirstWaypoint()
    {
        transform.position = _waypoints[0].position;
        _currentIndex = 0;
        _direction = ForwardDirection;
    }

    private Transform GetCurrentWaypoint()
    {
        return _waypoints[_currentIndex];
    }

    private bool HasReachedWaypoint(float sqrDistance)
    {
        float reachSqr = _reachDistance * _reachDistance;

        return sqrDistance <= reachSqr;
    }

    private bool IsLastWaypoint()
    {
        return _currentIndex == _waypoints.Count - 1;
    }

    //private bool IsFirstWaypoint()
    //{
    //    return _currentIndex == 0;
    //}

    private void MoveTowards(Vector3 toPoint)
    {
        if (toPoint == Vector3.zero)
            return;

        Vector3 direction = toPoint.normalized;
        transform.position += direction * (_speed * Time.deltaTime);
    }

    private void AdvanceToNextWaypoint()
    {
        if (_waypoints.Count <= 1)
            return;

        switch (_mode)
        {
            case PathTraversalMode.Loop:
                AdvanceLoop();
                break;

            case PathTraversalMode.PingPong:
                AdvancePingPong();
                break;

            case PathTraversalMode.SinglePass:
                AdvanceSinglePass();
                break;
        }
    }

    private void AdvanceLoop()
    {
        _currentIndex++;

        if (_currentIndex >= _waypoints.Count)
            _currentIndex = 0;
    }

    private void AdvancePingPong()
    {
        _currentIndex += _direction;

        if (_currentIndex >= _waypoints.Count)
        {
            _currentIndex = _waypoints.Count - 2;
            _direction = BackwardDirection;
        }
        else if (_currentIndex < 0)
        {
            _currentIndex = 1;
            _direction = ForwardDirection;
        }
    }

    private void AdvanceSinglePass()
    {
        if (IsLastWaypoint())
        {
            _isMoving = false;

            return;
        }

        _currentIndex++;
    }
}