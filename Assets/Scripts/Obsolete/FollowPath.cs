using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour {

    public enum FollowType {
        MoveTowards,
        Lerp
    }

    public FollowType type = FollowType.MoveTowards;
    public PathDefinition path;
    public float speed = 1;
    public float maxDistanceToGoal = .1f;

    private IEnumerator<Transform> _currentPoint;

    public void Start() {
        if (path == null) {
            Debug.LogError("Path cannot be null", gameObject);
            return;
        }

        _currentPoint = path.GetPathEnumerator();
        _currentPoint.MoveNext();

        if (_currentPoint.Current == null)
            return;

        transform.position = _currentPoint.Current.position;
    }

    public void Update() {
        if (_currentPoint == null || _currentPoint.Current == null)
            return;

        if (type == FollowType.MoveTowards)
            transform.position = Vector2.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
        else if (type == FollowType.Lerp)
            transform.position = Vector2.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);

        var distanceSquared = (transform.position - _currentPoint.Current.position).sqrMagnitude;
        if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal)
            _currentPoint.MoveNext();
    }

}
