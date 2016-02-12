using UnityEngine;
using System.Collections;
/// <summary>
/// This class acts as the controller for physical movement for entities. It is essentially a replacement for the Rigidbody2D component.
/// </summary>
public class CharacterController2D : MonoBehaviour, IPushable {

    // "Skin" is how far inside the player do the rays begin?
    private const float SkinWidth = .01f;
    private const int TotalHorizontalRays = 8;
    private const int TotalVerticalRays = 4;

    private static readonly float SlopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

    // What do we collide with?
    public LayerMask PlatformMask;
    public LayerMask MovingPlatformMask;
    public LayerMask OneWayMask;
    public ControllerParameters2D DefaultParameters;

    public ControllerState2D State { get; private set; }
    public Vector2 Velocity { get { return _velocity; } }
    public bool HandleCollisions { get; set; }
    // If Parameters != null, return _overrideParameters, else return DefaultParameters
    public ControllerParameters2D Parameters { get { return OverrideParameters ?? DefaultParameters; } }
    public ControllerParameters2D OverrideParameters { get; set; }
    public GameObject StandingOn { get; private set; }
    public Vector3 PlatformVelocity { get; private set; }
    public bool CanJump
    {
        get
        {
            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpAnywhere)
                return _jumpIn <= 0;

            if (Parameters.JumpRestrictions == ControllerParameters2D.JumpBehavior.CanJumpOnGround)
                return State.IsGrounded;

            return false;
        }
    }

    private Vector2 _velocity;
    private Vector2 _pusherVelocity;
    private Transform _transform;
    private Vector3 _localScale;
    private BoxCollider2D _boxCollider;

    private float _jumpIn;
    private GameObject _lastStandingOn;

    private Vector3
        _activeGlobalPlatformPoint,
        _activeLocalPlatformPoint;

    private Vector3
        _raycastTopLeft,
        _raycastBottomRight,
        _raycastBottomLeft;

    private float
        _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;

    public void Awake() {
        HandleCollisions = true;
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();

        // Store the actual width of the collider taking into account any scaling distortion
        // Then divide by the total number of vertical rays minus 1 to get the horizontal distance between rays
        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        // Same algorithm as above, but this computes vertical distance between rays.
        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
    }

    /// <summary>
    /// Adds a Vector2 to the entity's velocity.
    /// </summary>
    /// <param name="force"> How much force to add?</param>
    public void AddForce(Vector2 force) {
        _velocity += force;
    }

    /// <summary>
    /// Sets the entity's velocity equal to a Vector2.
    /// </summary>
    /// <param name="force"> The new velocity</param>
    public void SetForce(Vector2 force) {
        _velocity = force;
    }

    /// <summary>
    /// The x component of the entity's velocity is set to a value.
    /// </summary>
    /// <param name="x"> The new x component value</param>
    public void SetHorizontalForce(float x) {
        _velocity.x = x;
    }

    /// <summary>
    /// The y component of the entity's velocity is set to a value.
    /// </summary>
    /// <param name="y"> The new y component value</param>
    public void SetVerticalForce(float y) {
        _velocity.y = y;
    }

    /// <summary>
    /// The entity's vertical velocity is set to its jump magnitude.
    /// </summary>
    public void Jump() {
        if (_jumpIn <= 0) {
            SetVerticalForce(Parameters.JumpMagnitude);
            _jumpIn = Parameters.JumpFrequency;
        }
    }

    /// <summary>
    /// The entity reacts to an external object pushing it.
    /// </summary>
    /// <param name="push">Direction and intensity of the push</param>
    public void PushHorizontal(float push) {
        if (push > 0)
            State.IsCollidingLeft = true;
        else
            State.IsCollidingRight = true;

        _pusherVelocity.x += push;
    }

    public void PushVertical(float push) {
        AddForce(new Vector2(0f, push));
    }

    public void LateUpdate() {
        _jumpIn -= Time.deltaTime;

        // Apply gravity.
        _velocity.y += Parameters.Gravity * Time.deltaTime;

        State.Reset();

        HandlePushing(_pusherVelocity * Time.deltaTime);

        HandlePlatformEffects();

        Move(Velocity * Time.deltaTime);
    }

    private void Move(Vector2 deltaMovement) {
        //var wasGrounded = State.IsCollidingBelow;
        //State.Reset();

        if (HandleCollisions) {

            HandlePlatforms();
            CalculateRayOrigins();

            if (Mathf.Abs(deltaMovement.x) > 0f) // was .001f before the collision fix
                MoveHorizontally(ref deltaMovement);

            MoveVertically(ref deltaMovement);

            // Handle horizontal collision with moving terrain from the right and left
            //CorrectHorizontalPlacement(ref deltaMovement, true);
            //CorrectHorizontalPlacement(ref deltaMovement, false);

            //CorrectVerticalPlacement(ref deltaMovement);
        }

        // If the entity is trapped between walls, don't perform the movement.
        if (State.IsCollidingAbove && State.IsCollidingBelow || State.IsCollidingLeft && State.IsCollidingRight)
            return;

        // The actual movement of the player is performed. Calculations were done prior to this point.
        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if (State.IsMovingUpSlope)
            _velocity.y = 0;

        // Remember: Platforms utilizing the ControllerStay2D method must account for the time between frames. (Example: conveyer belt speed * Time.deltaTime)  
        if (StandingOn != null) {
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);

            if (_lastStandingOn != StandingOn)
                _lastStandingOn = StandingOn;

        } else if (_lastStandingOn != null) {
            _lastStandingOn = null;
        }
    }

    private void HandlePushing(Vector2 deltaMovement) {
        CalculateRayOrigins();
        if (Mathf.Abs(deltaMovement.x) > 0) {
            MoveHorizontally(ref deltaMovement);
            Debug.Log("being pushed");
        }

        _transform.Translate(deltaMovement, Space.World);
        _pusherVelocity = Vector2.zero;
    }
    /// <summary>
    /// If the entity is standing on a platform and that platform has moved, adjust the player's position accordingly.
    /// The raycasting portions check to make sure the entity is not passing through a wall.
    /// </summary>
    private void HandlePlatforms() {
        CalculateRayOrigins();
        if (StandingOn != null) {
            var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

            if (moveDistance != Vector3.zero) {
                Debug.Log("on a moving platform");
                if (moveDistance.x != 0f) {
                    var isMovingRight = moveDistance.x > 0f ? true : false;
                    // Insert vertical raycasting here
                    var rayDistance = Mathf.Abs(moveDistance.x) + SkinWidth;
                    var rayDirection = isMovingRight ? Vector2.right : Vector2.left;
                    var rayOrigin = isMovingRight ? _raycastBottomRight : _raycastBottomLeft;

                    // used to be for(var i = 0; i < TotalHorizontalRays; i++)
                    for (var i = 1; i < TotalHorizontalRays - 1; i++) {
                        var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

                        Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.magenta);
                        var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
                        if (!rayCastHit)
                            continue;

                        //if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                        //   break;

                        // Furthest we can move without hitting an obstacle
                        moveDistance.x = rayCastHit.point.x - rayVector.x;
                        // Small optimization that shortens rayDistance to what we just hit
                        rayDistance = Mathf.Abs(moveDistance.x);

                        if (isMovingRight) {
                            moveDistance.x -= SkinWidth;
                            State.IsCollidingRight = true;
                        } else {
                            moveDistance.x += SkinWidth;
                            State.IsCollidingLeft = true;
                        }

                        if (rayDistance < SkinWidth + .0001f)
                            break;
                    }
                }
                // Note: Here we only take upwards vertical movement into account since the entity is guaranteed to be standing on a platform.
                if (moveDistance.y > 0f) {
                    var isMovingUp = moveDistance.y > 0f ? true : false;
                    // Insert horizontal raycasting here
                    var rayDistance = Mathf.Abs(moveDistance.y) + SkinWidth;

                    for (var i = 1; i < TotalVerticalRays - 1; i++) {
                        var rayVector = new Vector2(_raycastTopLeft.x + (i * _horizontalDistanceBetweenRays), _raycastTopLeft.y);

                        var rayCastHit = Physics2D.Raycast(rayVector, Vector2.up, rayDistance, PlatformMask);
                        if (!rayCastHit)
                            continue;

                        moveDistance.y = rayCastHit.point.y - rayVector.y;
                        rayDistance = Mathf.Abs(moveDistance.y);

                        if (isMovingUp) {
                            moveDistance.y -= SkinWidth;
                            State.IsCollidingAbove = true;
                        }

                        if (rayDistance < SkinWidth + .0001f)
                            break;
                    }
                }
                transform.Translate(moveDistance, Space.World);
            }
            PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
        } else
            PlatformVelocity = Vector3.zero;

        StandingOn = null;

    }

    /// <summary>
    /// Send messages ControllerEnter2D, ControllerStay2D, and ControllerExit2D to necessary platforms.
    /// </summary>
    private void HandlePlatformEffects() {
        if (StandingOn != null) {
            if (_lastStandingOn != StandingOn) {
                if (_lastStandingOn != null)
                    _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

                StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
            } else if (StandingOn != null)
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);

        } else if (_lastStandingOn != null) {
            _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
        }
    }
    /*
    // Adjusts the entity's movement based on moving platforms from the left and right.
    private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight) {
        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
        var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;

        if (isRight)
            rayOrigin.x -= (halfWidth - SkinWidth);
        else
            rayOrigin.x += (halfWidth - SkinWidth);

        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;

        for (int i = 1; i < TotalHorizontalRays - 1; i++) {
            //The following line used to be ...  var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            // Taking out deltaMovement.x seems to have fixed the bouncing behavior against walls!
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);

            if (!raycastHit)
                continue;

            offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));

            if (isRight)
                State.IsCollidingRight = true;
            else if (!isRight)
                State.IsCollidingLeft = true;
        }
    
        // Performs the pushing of the player 
        deltaMovement.x += offset;
    }
    */
    /*
    // Account for the correct vertical placement of the entity with respect to moving platforms above.
    public void CorrectVerticalPlacement(ref Vector2 deltaMovement) {
        var halfWidth = (_boxCollider.size.y * _localScale.y) / 2f;
        var rayOrigin = _raycastTopLeft;

        rayOrigin.y -= (halfWidth - SkinWidth);

        var offset = 0f;

        for (int i = 1; i < TotalVerticalRays - 1; i++) {

            var rayVector = new Vector2( deltaMovement.x + rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);

            var raycastHit = Physics2D.Raycast(rayVector, Vector2.up, halfWidth, PlatformMask);
            //Debug.DrawRay(rayVector, Vector2.up * halfWidth, Color.red);
            if (!raycastHit)
                continue;

            offset = raycastHit.point.y - _transform.position.y - halfWidth;

            State.IsCollidingAbove = true;
        }

        deltaMovement.y += offset;
    }
    */
    /// <summary>
    /// Determines where the rays will be raycasted from. This is based on the entity's box collider.
    /// </summary>
    private void CalculateRayOrigins() {
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
        var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

        // These 3 vectors contain the origin points needed.
        _raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
    }

    private void MoveHorizontally(ref Vector2 deltaMovement) {
        var isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;

        for (var i = 0; i < TotalHorizontalRays; i++) {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            //Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);

            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            if (!rayCastHit)
                continue;

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;

            // Furthest we can move without hitting an obstacle
            deltaMovement.x = rayCastHit.point.x - rayVector.x;
            // Small optimization that shortens rayDistance to what we just hit
            rayDistance = Mathf.Abs(deltaMovement.x);

            if (isGoingRight) {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            } else {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }

            if (rayDistance < SkinWidth + 0f) //was .0001f
                break;
        }
    }

    private void MoveVertically(ref Vector2 deltaMovement) {
        var isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
        // If not going up, consider one way platforms as well!
        var maskToUse = isGoingUp ? PlatformMask.value : PlatformMask | OneWayMask;

        rayOrigin.x += deltaMovement.x;
        var standingOnDistance = float.MaxValue;
        for (var i = 0; i < TotalVerticalRays; i++) {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.blue);

            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, maskToUse);
            if (!raycastHit)
                continue;

            if (!isGoingUp) {
                var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
                if (verticalDistanceToHit < standingOnDistance) {
                    standingOnDistance = verticalDistanceToHit;
                    StandingOn = raycastHit.collider.gameObject;

                }
            }

            deltaMovement.y = raycastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);

            if (isGoingUp) {
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            } else {
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }

            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;

            if (rayDistance < SkinWidth + .0001f)
                break;
        }
    }

    private void HandleVerticalSlope(ref Vector2 deltaMovement) {
        var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
        var direction = -Vector2.up;

        var slopeDistance = SlopeLimitTangent * (_raycastBottomRight.x - center);
        var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);

        //Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
        var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        if (!raycastHit)
            return;

        var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
        if (!isMovingDownSlope)
            return;

        var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
        // Checks for perpendicular surface
        if (Mathf.Abs(angle) < .0001f)
            return;

        State.IsMovingDownSlope = true;
        State.SlopeAngle = angle;
        deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight) {
        if (Mathf.RoundToInt(angle) == 90)
            return false;

        // Is the slope too steep to move up?
        if (angle > Parameters.SlopeLimit) {
            deltaMovement.x = 0;
            return true;
        }

        if (deltaMovement.y > .07f)
            return true;

        deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;
        return true;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;

        OverrideParameters = parameters.Parameters;
    }

    public void OnTriggerExit2D(Collider2D other) {
        var parameters = other.gameObject.GetComponent<ControllerPhysicsVolume2D>();
        if (parameters == null)
            return;

        OverrideParameters = null;
    }
}
