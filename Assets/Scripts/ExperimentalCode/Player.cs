using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
// TODO think about adding Health Manager separately ?
public class Player : MonoBehaviour, ITakeDamage {
    private bool _isFacingRight;
    private CharacterController2D _controller;
    private Animator _animator;
    private BuffManager buffManager;
    private ItemManager itemManager;
    private SpriteRenderer playerSprite;
    private float _normalizedHorizontalSpeed;

    // How fast can the player move?
    public float MaxSpeed = 8;

    // How quickly can the player's velocity change?
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;
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

    // How long until the player can shoot next projectile?
    //private float _canFireIn;

    public void Awake() {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        buffManager = GetComponent<BuffManager>();
        itemManager = FindObjectOfType<ItemManager>();
        playerSprite = GetComponent<SpriteRenderer>();
        //TODO maybe find a better way to handle audio source organization.
        ouchEffect = GetComponent<AudioSource>();  
        _isFacingRight = transform.localScale.x > 0;

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
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        HandleAnimation();
    }
    // TODO add death animation?
    public void Kill() {
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;
        Health = 0;

        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void RespawnAt(Transform spawnPoint) {
        if (!_isFacingRight)
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
        if (Input.GetKey(KeyCode.RightArrow)) {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
                Flip();
        } else {
            _normalizedHorizontalSpeed = 0;
        }

        if (_controller.CanJump && Input.GetButtonDown("Jump")) {
            _controller.Jump();
        }

        if (Input.GetButtonDown("Respawn"))
            LevelManagerProto.Instance.KillPlayer();

    }

    private void HandleAnimation() {

        _animator.SetFloat("playerSpeed", _controller.Velocity.magnitude > .5f ? 1 : 0);
        _animator.SetBool("Ground", _controller.StandingOn != null);


    }
    // TODO merge with current projectile system.
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
        _isFacingRight = transform.localScale.x > 0;
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
