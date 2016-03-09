using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The Player class handles actions specific to the player. A lot of these actions interact with the CharacterController2D class.
/// </summary>
public class Player : MonoBehaviour, ITakeDamage {

    // References to components.
    private CharacterController2D _controller;
    private Animator _animator;
    private SpriteRenderer playerSprite;
    private BoxCollider2D _collider;
    private float _normalizedHorizontalSpeed;
    private float _normalizedVerticalSpeed;

    // How fast can the player move?
    public float MaxSpeed = 8;

    // How quickly can the player's velocity change?
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;
    [SerializeField]
    private Vector2 knockbackMagnitudes;
    [SerializeField]
    private float maxJumpDuration;
    [SerializeField]
    private ControllerParameters2D whileJumpingParameters;
    public bool IsFacingRight { get; private set; }
    // Is the user trying to move the character?
    public bool IsKeyboardInput { get; set; }
    public int MaxHealth = 3;

    public int invulnerabilityCap;
    public Transform ProjectileFireLocation;

    // Health information
    public int Health { get; private set; }
    public bool IsDead { get; private set; }
    public Toggle[] hearts;
    public bool IsInvulnerable { get; private set; }
    public AudioSource ouchEffect;          // On damage effect

    // Status information
    private bool knockbackActive = false;

    // State information specific to the player. This code is here instead of CharacterController since as of right now, climbing is restricted to being
    // something only the player can do.


    // Tracks how many ladder colliders the player is inside of. This is necessary for adjacent ladders to function properly.
    public int LadderColliderCount { get; set; }

    // Will hold all of the OnTriggerExit2D methods of the colliders the player is currently in. (This is so they get called even when the player is disabled upon death.)
    public Action<Collider2D> ExitAllTriggers;

    // Climbing fields/properties
    public ControllerParameters2D climbingParameters;
    public bool CanClimb { get; set; }
    private bool isClimbing;

    public bool IsClimbing
    {
        get
        {
            return isClimbing;
        }
        set
        {
            if (value != isClimbing) {

                if (value)
                    _controller.OverrideParameters = climbingParameters;
                else {
                    _controller.OverrideParameters = null;
                    _animator.speed = 1;
                    _controller.SetVerticalForce(0f);
                }

                isClimbing = value;
                _animator.SetBool("isClimbing", IsClimbing);
            }
        }
    }

    public void Awake() {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        //TODO maybe find a better way to handle audio source organization.
        ouchEffect = GetComponent<AudioSource>();
        IsFacingRight = transform.localScale.x > 0;
        FullHealth();
    }

    public void Update() {

        if (_controller.IsBeingCrushed)
            LevelManager.Instance.KillPlayer();

        if (!IsDead && !knockbackActive)
            HandleInput();

        // var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        // Horizontal force to be added is determined by interpolating current velocity and the desired direction times the max speed.
        // Movement factor determines distance per frame based on location ie ground, air, water, etc...
        if (IsDead) {
            _controller.SetHorizontalForce(0);

        } else if (IsClimbing) {

            _controller.SetForce(new Vector2(_normalizedHorizontalSpeed * MaxSpeed / 2.5f, _normalizedVerticalSpeed * MaxSpeed / 2.5f));
            // Handles the intersection of ladders with the ground
            if (_controller.State.IsCollidingBelow && _normalizedVerticalSpeed == -1)
                IsClimbing = false;
        } else if (!knockbackActive) {
            _controller.SetHorizontalForce(_normalizedHorizontalSpeed * MaxSpeed);
            // Acceleration Movement: Currently disabled until a solution is found to make it behave with the rest of the physics engine.
            //_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, movementFactor * Time.deltaTime));
        }

        // NOTE: 1/3/2016 had to fix issues with vsync. Removed multiplying movementFactor by Time.deltaTime and used smaller values for the acceleration on ground/air.

        if (!IsDead)
            HandleAnimation();
    }

    /// <summary>
    /// Kills the player.
    /// </summary>
    public void Kill() {
        CancelInvoke("SpriteToggle");
        _animator.SetBool("isHurt", true);
        CancelJump();
        StopAllCoroutines();
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        if (ExitAllTriggers != null)
            ExitAllTriggers(_collider);
        IsDead = true;
        Health = 0;

        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].isOn = false;
        }
        GetComponent<SpriteRenderer>().enabled = true;

        knockbackActive = false;
        IsInvulnerable = false;
    }

    /// <summary>
    /// Respawns the player at a certain position in the world.
    /// </summary>
    /// <param name="spawnPoint">Where should the player respawn?</param>
    public void RespawnAt(Transform spawnPoint) {
        if (!IsFacingRight)
            Flip();

        IsDead = false;
        GetComponent<Collider2D>().enabled = true;
        _controller.HandleCollisions = true;
        _controller.SetForce(Vector2.zero);
        playerSprite.enabled = true;
        FullHealth();

        transform.position = spawnPoint.position;
    }

    // Deduct the amount of damage taken from player health, then change the UI to represent that.
    // Check if this kills the player. This code will not execute if the player is currently invulnerable.
    // TODO Make an animation that plays for a bit upon getting hit.
    public void TakeDamage(int damage, GameObject instigator) {
        if (!IsInvulnerable) {
            CancelJump();
            ouchEffect.Play();

            for (int i = Health - 1; i > Health - damage - 1; i--) {
                if (i < 0 || hearts[i] == null)
                    break;
                hearts[i].isOn = false;

            }

            Health -= damage;

            if (Health <= 0)
                LevelManager.Instance.KillPlayer();
            else
                BuffInvulnerability();
        }
    }

    public void FullHealth() {
        Health = MaxHealth;
        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].isOn = true;
        }
    }
    // TODO Refactor to use GetAxisRaw
    private void HandleInput() {
        _normalizedHorizontalSpeed = Input.GetAxisRaw("Horizontal");
        _normalizedVerticalSpeed = Input.GetAxisRaw("Vertical");

        IsKeyboardInput = _normalizedHorizontalSpeed == 0 ? false : true;

        if (_normalizedHorizontalSpeed == 1) {
            if (!IsFacingRight)
                Flip();
        } else if (_normalizedHorizontalSpeed == -1)
            if (IsFacingRight)
                Flip();

        // IMPORTANT: Rework jumping to a variable jump dependent on how long the jump key is held down.
        //  When the Jump button is pressed and the player isn't currently jumping, start a coroutine that will execute the jumping process.
        //  To avoid the jetpack feel:
        //      Gravity will be turned off during the jump routine. This is because an artificial gravity-like feel is incorporated into the routine itself.
        //      Establish a maximum jump time. When this time is reached OR if the player releases the button.
        //      The earliest frames of the jump will be the strongest in terms of force. As the jump goes on, the force will decrease, reaching 0 at maxTime.
        //  The jumping coroutine will exit when either the player releases the jump button or the maxTime has passed.
        if (Input.GetButtonDown("Jump")) {

            if (_controller.State.IsGrounded)
                StartCoroutine("JumpCo");

            if (IsClimbing)
                IsClimbing = false;
        }

        if (Input.GetButtonDown("Respawn"))
            LevelManager.Instance.KillPlayer();

        // This is simply for convenience while testing the game in the editor.
#if UNITY_EDITOR
        if (Input.GetKeyDown("t") && !IsDead)
            FullHealth();
#endif


    }

    private void HandleAnimation() {

        _animator.SetFloat("playerSpeed", IsKeyboardInput ? 1 : 0);     //previously also had the condition of _velocity.x > .5f. I do not believe this was needed.

        // If climbing and moving, the animation speed should be normal or "1". If not moving, then the speed should be 0.
        if (IsClimbing)
            if (_controller.Velocity.magnitude > .5f)
                _animator.speed = 1;
            else
                _animator.speed = 0;


        _animator.SetBool("Ground", _controller.State.IsCollidingBelow);
        _animator.SetBool("isHurt", knockbackActive);

    }

    // Flips the player over the y axis by negating the current x property of local scale
    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        IsFacingRight = transform.localScale.x > 0;
    }

    private IEnumerator JumpCo () {

        // Enabling whileJumpingParameters to turn off gravity.
        var jumpTimer = 0f;
        _controller.OverrideParameters = whileJumpingParameters;

        // Continue jumping if the button is being pressed, the jump hasn't exceeded max duration, not colliding against a ceiling, and still using whileJumpingParameters.
        // Note: Canceling this coroutine when taking damage/dying/ occurs on a per case basis by calling the CancelJump method.
        while (Input.GetButton("Jump") && jumpTimer < maxJumpDuration && !_controller.State.IsCollidingAbove && _controller.OverrideParameters == whileJumpingParameters) {
            float proportionCompleted = jumpTimer / maxJumpDuration;
            //IMPORTANT: Continue working on implementing the new jumping system!!!!!
        }
        _controller.OverrideParameters = null;
        yield return null;
    }

    /// <summary>
    /// Call when the JumpCo Coroutine is interrupted.
    /// </summary>
    private void CancelJump () {
        StopCoroutine("JumpCo");
        _controller.OverrideParameters = null;
    }

    /// <summary>
    /// Initiates the coroutine responsible for knocking the player away from a point
    /// </summary>
    /// <param name="instigatorPosition">Location of the entity knocking the player back</param>
    public void Knockback(Vector2 instigatorPosition) {
        if (!knockbackActive) {
            knockbackActive = true;
            IsClimbing = false;
            StartCoroutine(KnockbackCo(instigatorPosition));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instigatorPosition"></param>
    /// <returns></returns>
    private IEnumerator KnockbackCo(Vector2 instigatorPosition) {

        Vector2 knockbackForce;

        // The player should be knocked away horizontally from whatever is knocking it back.
        knockbackForce.x = instigatorPosition.x > transform.position.x ? -knockbackMagnitudes.x : knockbackMagnitudes.x;

        // If standing on the ground, always apply an upwards knockback.
        if (_controller.State.IsCollidingBelow)
            knockbackForce.y = knockbackMagnitudes.y;
        // Otherwise knock the player in the opposite vertical direction of the instigator.
        else
            knockbackForce.y = instigatorPosition.y > transform.position.y ? -knockbackMagnitudes.y : knockbackMagnitudes.y;

        _controller.SetForce(knockbackForce);

        // Skip a frame so the player can actually move from the ground.
        yield return null;

        while (!_controller.State.IsCollidingBelow) {
            yield return null;
        }

        //Debug.Log("knockback ended");
        knockbackActive = false;
    }
    public void BuffInvulnerability() {
        StartCoroutine("BuffInvulnerabilityCo");
    }

    /// <summary>
    /// The player is invulnerable and the sprite will flicker for the duration
    /// </summary>
    /// <returns>WaitForSeconds or nothing</returns>
    private IEnumerator BuffInvulnerabilityCo()             //Player cannot be damaged while this is active. Sprite will toggle on/off as an indicator
    {
        IsInvulnerable = true;
        InvokeRepeating("SpriteToggle", 0f, .1f);

        while (knockbackActive) {
            Debug.Log("knockback is still active.");
            yield return null;
        }
        yield return new WaitForSeconds(invulnerabilityCap);
        IsInvulnerable = false;
        CancelInvoke("SpriteToggle");
        playerSprite.enabled = true;


    }
    /// <summary>
    /// The SpriteRenderer component is toggled on or off
    /// </summary>
    private void SpriteToggle() {
        if (IsInvulnerable)
            playerSprite.enabled = !playerSprite.enabled;
    }
}
