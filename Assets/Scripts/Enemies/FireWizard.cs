using System;
using UnityEngine;

public class FireWizard : Enemy {


    public Projectile fireball;
    private SimpleProjectile activeFireball;
    private CharacterController2D controller;
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
        direction = Vector2.left;

        controller = GetComponent<CharacterController2D>();
        playerTransform = FindObjectOfType<Player>().transform;
        boxCollider = GetComponent<BoxCollider2D>();

        // This fireball will be pooled as the only one ever in use for this enemy.
        activeFireball = (SimpleProjectile)Instantiate(activeFireball, transform.position, transform.rotation);
        activeFireball.enabled = false;

        // Calculate the detection rectangle used to search for the player.
        var topLeftOrigin = new Vector2(boxCollider.bounds.min.x - playerDetectionLength, boxCollider.bounds.max.y);
        var size = new Vector2(playerDetectionLength + boxCollider.bounds.extents.x, boxCollider.bounds.size.y);

        playerDetectionBox = new Rect(topLeftOrigin, size);

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
        activeFireball.transform.position = firePoint.position;

    }


    protected override void Flip() {
        base.Flip();
        playerDetectionBox.x = direction == Vector2.right ? playerDetectionBox.x + playerDetectionLength * 2 : playerDetectionBox.x - playerDetectionLength * 2;
    }
}