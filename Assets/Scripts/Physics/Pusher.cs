using UnityEngine;
using System.Collections.Generic;

//IMPORTANT: Keep an eye on this class. I died one time messing around with the pushing platforms but could not replicate it after repeated attempts.
/// <summary>
/// Any GameObject with this component will push entities with the IPushable interface on any of their components upon collision.
/// </summary>
public class Pusher : MonoBehaviour {

    private const float SkinWidth = .01f;
    private const float _desiredDistanceBetweenRays = .2f;

    private float TotalHorizontalRays;
    private float TotalVerticalRays;
    private float _horizontalDistanceBetweenRays;
    private float _verticalDistanceBetweenRays;

    private Vector3
        _raycastTopLeft,
        _raycastBottomRight,
        _raycastBottomLeft;

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

        // Store the actual width of the collider taking into account any scaling distortion
        // Then divide by the total number of vertical rays minus 1 to get the horizontal distance between rays
        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        TotalVerticalRays = Mathf.CeilToInt(colliderWidth / _desiredDistanceBetweenRays + 1);
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        // Same algorithm as above, but this computes vertical distance between rays.
        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        TotalHorizontalRays = Mathf.CeilToInt(colliderHeight / _desiredDistanceBetweenRays + 1);
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);

        positionLastFrame = _transform.position;
    }

    void Update() {

        Vector3 deltaMovement = _transform.position - positionLastFrame;
        CalculateRayOrigins(deltaMovement);

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
    private void CalculateRayOrigins(Vector3 deltaMovement) { //IMPORTANT: Look into other properties of BoxCollider2D to find a better way to represent this in generic terms.

        _raycastTopLeft = _boxCollider.bounds.center - deltaMovement + new Vector3(-_boxCollider.bounds.extents.x + SkinWidth, _boxCollider.bounds.extents.y - SkinWidth);
        _raycastBottomRight = _boxCollider.bounds.center - deltaMovement + new Vector3(_boxCollider.bounds.extents.x - SkinWidth, -_boxCollider.bounds.extents.y + SkinWidth);
        _raycastBottomLeft = _boxCollider.bounds.center - deltaMovement + new Vector3(-_boxCollider.bounds.extents.x + SkinWidth, -_boxCollider.bounds.extents.y + SkinWidth);
    }

    /// <summary>
    /// Checks to see if anything needs to be pushed out of the way vertically. (Only handles downward movement for now)
    /// </summary>
    /// <param name="deltaMovement">Represents the change in movement of this GameObject this frame.</param>
    private void CheckVerticalCollision(Vector2 deltaMovement) {
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;

        var objectsHit = new HashSet<GameObject>(); 

        for (int i = 0; i < TotalVerticalRays; i++) {
            var rayVector = new Vector2(_raycastBottomLeft.x + (i * _horizontalDistanceBetweenRays), _raycastBottomLeft.y);

            var rayCastHit = Physics2D.RaycastAll(rayVector, Vector2.down, rayDistance);
            //Debug.DrawRay(rayVector, Vector2.down * rayDistance, Color.green);
            if (rayCastHit.Length == 0)
                return;

            foreach (var hit in rayCastHit) {
                if (!objectsHit.Contains(hit.collider.gameObject)) {
                    var pushable = (IPushable)hit.collider.GetComponent(typeof(IPushable));
                    if (pushable != null) {
                        var amountToPush = rayVector.y + deltaMovement.y - hit.point.y - SkinWidth;
                        pushable.PushVertical(amountToPush / Time.deltaTime);
                        objectsHit.Add(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check to see if anything needs to be pushed out of the way horizontally.
    /// </summary>
    /// <param name="deltaMovement">Represents the change in movement of this GameObject this frame.</param>
    private void CheckHorizontalCollision(Vector2 deltaMovement) {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        // Track the hit GameObjects so they aren't pushed twice in one frame.
        var objectsHit = new HashSet<GameObject>();

        for (int i = 0; i < TotalHorizontalRays; i++) {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

            var rayCastHit = Physics2D.RaycastAll(rayVector, rayDirection, rayDistance);
            //Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.cyan);
            if (rayCastHit.Length == 0)
                return;


            // For each object hit with a component that implements IPushable, apply the proper amount of force to simulate pushing.
            foreach (var hit in rayCastHit) {
                if (!objectsHit.Contains(hit.collider.gameObject)) {
                    var pushable = (IPushable)hit.collider.GetComponent(typeof(IPushable));
                    if (pushable != null) {
                        var amountToPush = rayVector.x + deltaMovement.x - hit.point.x;
                        amountToPush = isGoingRight ? amountToPush + SkinWidth : amountToPush - SkinWidth;
                        Debug.Log(amountToPush);
                        pushable.PushHorizontal(amountToPush / Time.deltaTime);
                        objectsHit.Add(hit.collider.gameObject);
                    }
                }
            }
        }

    }
}
