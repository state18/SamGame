using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Any GameObject with this component will push entities with the IPushable interface on any of their components upon collision.
/// </summary>
public class Pusher : MonoBehaviour { // IMPORTANT: The math is probably not 100% correct because sometimes the player will die when walking into oncoming pusher. FIX THIS

    private const float BoxThickness = .02f;

    private Vector2
        _boxcastLeft,
        _boxcastRight,
        _boxcastUp,
        _boxcastDown;

    // Keeping track of the last position will tell us how much this object has moved.
    private Vector3 positionLastFrame;
    private BoxCollider2D _boxCollider;
    private Vector3 _localScale;
    private Transform _transform;


    // Use this for initialization
    void Start() {
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        positionLastFrame = _transform.position;
    }

    void Update() {

        Vector2 deltaMovement = _transform.position - positionLastFrame;
        CalculateBoxOrigins(deltaMovement);

        if (deltaMovement.y < 0)
            CheckVerticalCollision(deltaMovement);

        if (Mathf.Abs(deltaMovement.x) > 0)
            CheckHorizontalCollision(deltaMovement);

        positionLastFrame = _transform.position;
    }

    /// <summary>
    /// The key points where the rays will originate are established based on the position before movement.
    /// Note: These points assume the origin of the transform is at the bottom left of the box collider!!!
    /// </summary>
    private void CalculateBoxOrigins(Vector3 deltaMovement) { //IMPORTANT: Look into other properties of BoxCollider2D to find a better way to represent this in generic terms.
        //var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y));
        //var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        /*
        // These 3 vectors contain the origin points needed.
        _boxcastLeft = positionLastFrame + new Vector3(BoxThickness /2f, center.y);
        _boxcastRight = positionLastFrame + new Vector3(size.x - BoxThickness /2f, center.y);
        _boxcastUp = positionLastFrame + new Vector3(center.x, center.y + size.y - BoxThickness /2f);
        _boxcastDown = positionLastFrame + new Vector3(center.x, center.y - size.y + BoxThickness /2f);
        */

        // Experimental code to replace the above code. It seems more generic (It doesn't rely on the Transform having a pivot point around a certain area)
        _boxcastLeft = _boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x - BoxThickness/2f, 0f) - deltaMovement;
        _boxcastRight = _boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x - BoxThickness/2f, 0f) - deltaMovement;
        _boxcastUp = _boxCollider.bounds.center + new Vector3(0f, _boxCollider.bounds.extents.y - BoxThickness/2f) - deltaMovement;
        _boxcastDown = _boxCollider.bounds.center - new Vector3(0f, _boxCollider.bounds.extents.y - BoxThickness/2f) - deltaMovement;
    }

    /// <summary>
    /// Checks to see if anything needs to be pushed out of the way vertically. (Only handles downward movement for now)
    /// </summary>
    /// <param name="deltaMovement">Represents the change in movement of this GameObject this frame.</param>
    private void CheckVerticalCollision(Vector2 deltaMovement) {
        var boxDistance = Mathf.Abs(deltaMovement.y);
        var boxDimensions = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.y) - .05f, BoxThickness);

        var boxCastHit = Physics2D.BoxCastAll(_boxcastDown, boxDimensions, 0f, Vector2.down, boxDistance);
        Debug.DrawLine(_boxcastDown, _boxcastUp, Color.red); // Just shows where the _boxcastDown and _boxcastUp points are

        if (boxCastHit.Length == 0)
            return;

        foreach (var hit in boxCastHit) {
            var pushable = (IPushable)hit.collider.GetComponent(typeof(IPushable));
            if (pushable != null) {
                var amountToPush = _boxcastDown.y + deltaMovement.y - hit.point.y;
                pushable.PushVertical(amountToPush / Time.deltaTime);
            }
        }
    }


    /// <summary>
    /// Check to see if anything needs to be pushed out of the way horizontally.
    /// </summary>
    /// <param name="deltaMovement">Represents the change in movement of this GameObject this frame.</param>
    private void CheckHorizontalCollision(Vector2 deltaMovement) {
        var isGoingRight = deltaMovement.x > 0;
        var boxDistance = Mathf.Abs(deltaMovement.x);
        var boxDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var boxOrigin = isGoingRight ? _boxcastRight : _boxcastLeft;
        var boxDimensions = new Vector2(BoxThickness, _boxCollider.size.y * Mathf.Abs(_localScale.y) - .05f);   //Think of the .05f as skin from the CharacterController2D.

        //Debug.DrawRay(rayVector, boxDirection * boxDistance, Color.red);
        var boxCastHit = Physics2D.BoxCastAll(boxOrigin, boxDimensions, 0f, boxDirection, boxDistance);
        Debug.DrawLine(_boxcastLeft, _boxcastRight, Color.magenta);

        if (boxCastHit.Length == 0)
            return;

        // For each object hit with a component that implements IPushable, apply the proper amount of force to simulate pushing.
        foreach (var hit in boxCastHit) {
            var pushable = (IPushable)hit.collider.GetComponent(typeof(IPushable));
            if (pushable != null) {
                var amountToPush = boxOrigin.x + deltaMovement.x - hit.point.x;
                Debug.Log(hit.point);
                //amountToPush = isGoingRight ? amountToPush + BoxThickness : amountToPush - BoxThickness;
                Debug.Log(amountToPush);
                pushable.PushHorizontal(amountToPush / Time.deltaTime);
            }
        }

    }
}
