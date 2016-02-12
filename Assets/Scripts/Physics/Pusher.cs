using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Any GameObject with this component will push entities with the IPushable interface on any of their components upon collision.
/// </summary>
public class Pusher : MonoBehaviour {

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

    [SerializeField]
    LayerMask layer;

    private float speed;
    // Use this for initialization
    void Start() {
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        positionLastFrame = _transform.position;
    }

    void Update() {
        CalculateBoxOrigins();
        var deltaMovement = _transform.position - positionLastFrame;
        //if (Mathf.Abs(deltaMovement.y) > 0)
        //CheckVerticalCollision(deltaMovement);

        if (Mathf.Abs(deltaMovement.x) > 0)
            CheckHorizontalCollision(deltaMovement);

        positionLastFrame = _transform.position;
    }

    /// <summary>
    /// The key points where the rays will originate are established based on the position before movement.
    /// </summary>
    private void CalculateBoxOrigins() {
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2f;
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        // These 3 vectors contain the origin points needed.
        _boxcastLeft = positionLastFrame + new Vector3(center.x - size.x + BoxThickness /2f, 0f);
        _boxcastRight = positionLastFrame + new Vector3(center.x + size.x - BoxThickness /2f, 0f);
        _boxcastUp = positionLastFrame + new Vector3(0f, center.y + size.y - BoxThickness /2f);
        _boxcastDown = positionLastFrame + new Vector3(0f, center.y - size.y + BoxThickness /2f);
    }

    /*
    private void CheckVerticalCollision(Vector2 deltaMovement) {
        var isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;

        rayOrigin.x += deltaMovement.x;
        var standingOnDistance = float.MaxValue;
        for (var i = 0; i < TotalVerticalRays; i++) {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.blue);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance);
            if (!raycastHit)
                continue;

            if (!isGoingUp) {
                var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
                if (verticalDistanceToHit < standingOnDistance) {
                    standingOnDistance = verticalDistanceToHit;

                }
            }

            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp)
                deltaMovement.y -= SkinWidth;

            else
                deltaMovement.y += SkinWidth;

            if (!isGoingUp && deltaMovement.y > .0001f)

                if (rayDistance < SkinWidth + .0001f)
                    break;
        }
    }
    */

    /// <summary>
    /// Check to see if anything needs to be pushed out of the way horizontally.
    /// </summary>
    /// <param name="deltaMovement">Represents the change in movement of this GameObject this frame.</param>
    private void CheckHorizontalCollision(Vector2 deltaMovement) {
        var isGoingRight = deltaMovement.x > 0;
        var boxDistance = Mathf.Abs(deltaMovement.x);
        var boxDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var boxOrigin = isGoingRight ? _boxcastRight : _boxcastLeft;
        var boxDimensions = new Vector2(BoxThickness, _boxCollider.size.y * Mathf.Abs(_localScale.y));

        //Debug.DrawRay(rayVector, boxDirection * boxDistance, Color.red);
        var boxCastHit = Physics2D.BoxCastAll(boxOrigin, boxDimensions, 0f, boxDirection, boxDistance);
        //Debug.DrawLine(_boxcastLeft, _boxcastRight, Color.magenta);
        //Debug.DrawLine(_boxcastDown, _boxcastUp, Color.red);
        if (boxCastHit.Length == 0)
            return;

        // For each object hit with a component that implements IPushable, apply the proper amount of force to simulate pushing.
        foreach (var hit in boxCastHit) {
            var pushable = (IPushable)hit.collider.GetComponent(typeof(IPushable));
            if (pushable != null) {
                var amountToPush = boxOrigin.x + deltaMovement.x - hit.point.x;
                amountToPush = isGoingRight ? amountToPush + BoxThickness /2f : amountToPush - BoxThickness /2f;
                pushable.PushHorizontal(amountToPush / Time.deltaTime);
                Debug.Log(amountToPush);
            }
        }

    }
}
