using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The FireWizard is an enemy that fires fireball projectiles at the player if they're within range. The FireWizard doesn't move.
/// </summary>
public class FireWizard : Enemy {

    public Projectile fireball;
    private SimpleProjectile activeFireball;
    private CharacterController2D controller;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Transform playerTransform;

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float fireCooldownTime;
    [SerializeField]
    private float playerDetectionLength;
    [SerializeField]
    private LayerMask playerMask;

    private float canFireIn;
    private Rect playerDetectionBox;

    void Start() {
        Initialize();
        startPosition = transform.position;
        direction = Vector2.left;

        controller = GetComponent<CharacterController2D>();
       
        playerTransform = FindObjectOfType<Player>().transform;
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        // Calculate the detection rectangle used to search for the player.
        var bottomLeftOrigin = new Vector2(boxCollider.bounds.min.x - playerDetectionLength, boxCollider.bounds.min.y);
        var size = new Vector2(playerDetectionLength + boxCollider.bounds.extents.x, boxCollider.bounds.size.y);

        playerDetectionBox = new Rect(bottomLeftOrigin, size);

    }

    void Update() {

        if (canFireIn > 0)
            canFireIn -= Time.deltaTime;

        if (playerTransform.position.x > transform.position.x && direction == Vector2.left || playerTransform.position.x < transform.position.x && direction == Vector2.right)
            Flip();

        var playerInRange = Physics2D.OverlapArea(new Vector2(playerDetectionBox.xMin, playerDetectionBox.yMax), new Vector2(playerDetectionBox.xMax, playerDetectionBox.yMin), playerMask);
        if (playerInRange && canFireIn <= 0)
            Fire();
    }

    void Fire() {

        canFireIn = fireCooldownTime;
        var activeFireball = (SimpleProjectile)Instantiate(fireball, firePoint.position, firePoint.rotation);
        activeFireball.Initialize(gameObject, direction);
        activeFireball.transform.localScale = activeFireball.transform.localScale * Mathf.Sign(transform.localScale.x);
        StartCoroutine(HandleAnimation());

    }

    /// <summary>
    /// Transition to the firing animation then wait for a small amount of time before switching back to the idle animation.
    /// </summary>
    /// <returns></returns>
    IEnumerator HandleAnimation () {
        anim.SetBool("isFiring", true);
        yield return new WaitForSeconds(.7f);
        anim.SetBool("isFiring", false);
        
    }
    
    protected override void Flip() {
        base.Flip();
        playerDetectionBox.x = direction == Vector2.right ? playerDetectionBox.x + playerDetectionLength : playerDetectionBox.x - playerDetectionLength;
    }

    void OnDisable () {
        StopAllCoroutines();
        //anim.SetBool("isFiring", false);
        canFireIn = 0f;
    }
}