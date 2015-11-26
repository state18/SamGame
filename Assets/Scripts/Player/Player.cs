using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
// TODO think about adding Health Manager separately ?
/// <summary>
/// The Player class handles actions specific to the player. A lot of these actions interact with the CharacterController2D class.
/// </summary>
public class Player : MonoBehaviour, ITakeDamage {
    
    private CharacterController2D _controller;
    private Animator _animator;
    private SpriteRenderer playerSprite;
    private float _normalizedHorizontalSpeed;
    private float _normalizedVerticalSpeed;

    // How fast can the player move?
    public float MaxSpeed = 8;

    // How quickly can the player's velocity change?
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;
    public bool IsFacingRight { get; private set; }
    // Is the user trying to move the character?
    public bool IsKeyboardInput { get; set; }
    public int MaxHealth = 3;
    public AudioSource ouchEffect;          // On damage effect
    public Projectile Projectile;            // TODO merge this with current projectile system.
    public float FireRate;
    public int invulnerabilityCap;
    public Transform ProjectileFireLocation;

    // Health information
    public int Health { get; private set; }
    public bool IsDead { get; private set; }
    public Toggle[] hearts;
    public bool IsInvulnerable { get; private set; }

    // State information specific to the player. This code is here instead of CharacterController since as of right now, climbing is restricted to being
    // something only the player can do.
    public ControllerParameters2D climbingParameters;
    public bool CanClimb { get; set; }
    private bool isClimbing;

    public bool IsClimbing {
        get {
            return isClimbing;
        }
        set {
            if (value)
                _controller.OverrideParameters = climbingParameters;
            else {
                _controller.OverrideParameters = null;
                _animator.speed = 1;
            }
            isClimbing = value;
            _animator.SetBool("isClimbing", IsClimbing);
        }
    }
    // How long until the player can shoot next projectile?
    //private float _canFireIn;

    public void Awake() {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        //TODO maybe find a better way to handle audio source organization.
        ouchEffect = GetComponent<AudioSource>();
        IsFacingRight = transform.localScale.x > 0;
        FullHealth();
    }

    public void Update() {

        if (!IsDead)
            HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        // Horizontal force to be added is determined by interpolating current velocity and the desired direction times the max speed.
        // Movement factor determines distance per frame based on location ie ground, air, water, etc...
        if (IsDead)
            _controller.SetHorizontalForce(0);
        else if (IsClimbing)
            _controller.SetForce(new Vector2(_normalizedHorizontalSpeed * MaxSpeed * 20 * Time.deltaTime, _normalizedVerticalSpeed * MaxSpeed * 20 * Time.deltaTime));
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        HandleAnimation();
    }

    /// <summary>
    /// Kills the player.
    /// </summary>
    public void Kill() {
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;
        Health = 0;

        GetComponent<SpriteRenderer>().enabled = false;
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
        GetComponent<SpriteRenderer>().enabled = true;
        FullHealth();

        transform.position = spawnPoint.position;
    }

    // Deduct the amount of damage taken from player health, then change the UI to represent that.
    // Check if this kills the player. This code will not execute if the player is currently invulnerable.
    // TODO Make an animation that plays for a bit upon getting hit.
    public void TakeDamage(int damage, GameObject instigator) {
        if (!IsInvulnerable) {
            ouchEffect.Play();

            for (int i = Health - 1; i > Health - damage - 1; i--) {
                if (i < 0 || hearts[i] == null)
                    break;
                hearts[i].isOn = false;

            }

            Health -= damage;

            if (Health <= 0)
                LevelManagerProto.Instance.KillPlayer();
            else
                BuffInvulnerability();


        }


    }

    public void FullHealth() {
        Health = MaxHealth;
        foreach (Toggle t in hearts) {
            t.isOn = true;
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

        if (_controller.CanJump && Input.GetButtonDown("Jump")) {
            _controller.Jump();
        }

        if (Input.GetButtonDown("Respawn"))
            LevelManagerProto.Instance.KillPlayer();
    }

    private void HandleAnimation() {

        _animator.SetFloat("playerSpeed",  Math.Abs(_controller.Velocity.x) >.5f && IsKeyboardInput ? 1 : 0);

        // If climbing and moving, the animation speed should be normal or "1". If not moving, then the speed should be 0.
        if (IsClimbing)
            if (_controller.Velocity.magnitude > .5f)
                _animator.speed = 1;
            else
                _animator.speed = 0;


        _animator.SetBool("Ground", _controller.StandingOn != null);

    }
    // TODO IMPORTANT! This shows how to properly use the new projectile system! Use this with pistol item.
    /*
    private void FireProjectile() {
        if (_canFireIn > 0)
            return;

        var direction = _isFacingRight ? Vector2.right : -Vector2.right;

        var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        projectile.Initialize(gameObject, direction, _controller.Velocity);

        _canFireIn = FireRate;
    }
    */

    // Flips the player over the y axis by negating the current x property of local scale
    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        IsFacingRight = transform.localScale.x > 0;
    }

    public void BuffInvulnerability() {
        StartCoroutine("BuffInvulnerabilityCo");        //calls coroutine below (because there is a yield statement)
    }

    IEnumerator BuffInvulnerabilityCo()             //Player cannot be damaged while this is active. Sprite will toggle on/off as an indicator
    {
        IsInvulnerable = true;
        InvokeRepeating("SpriteToggle", 0f, .1f);

        yield return new WaitForSeconds(invulnerabilityCap);
        IsInvulnerable = false;
        CancelInvoke("SpriteToggle");
        playerSprite.enabled = true;


    }

    public void SpriteToggle()                          //Flickers the player sprite on/off each time it is called
    {
        if (IsInvulnerable) {
            playerSprite = GetComponent<SpriteRenderer>();

            playerSprite.enabled = !playerSprite.enabled;
        }
    }
}
