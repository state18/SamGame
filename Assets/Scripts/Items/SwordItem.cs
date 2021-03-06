﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This item allows the player to perform a melee attack.
/// </summary>
public class SwordItem : Item {

    public Transform handStart;
    public Transform handFinish;
    private CharacterController2D playerController;
    private Player player;

    Animator playerAnim;

    public float distance;
    private float cooldown = 0f;
    public float cooldownTime;
    public float enemyKnockback;
    private int ongoingPunches = 0;
    public int damageToGive;
    public Vector2 boxSize;
    public int hitDirection;

    public LayerMask punchable;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
        playerAnim = player.GetComponent<Animator>();
        playerController = player.GetComponent<CharacterController2D>();

    }

    // Update is called once per frame
    void Update() {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Perform a melee attack if off cooldown
    /// </summary>
    public override void Use() {
        if (cooldown <= 0 && !player.IsClimbing) {
            GetComponent<AudioSource>().Play();
            StartCoroutine("UseCo");

        }
    }
    public IEnumerator UseCo() {
        RaycastHit2D[] hit;
        cooldown = cooldownTime;
        ongoingPunches++;
        playerAnim.SetBool("isPunching", true);

        float adjustedDistance = (Mathf.Abs(playerController.Velocity.x) > 0) ? distance + .33f : distance;
        Vector2 origin = Vector2.Lerp(new Vector2(handStart.position.x, handStart.position.y), new Vector2(handFinish.position.x, handFinish.position.y), .5f);

        if (handStart.parent.transform.localScale.x > 0) {

            hit = Physics2D.BoxCastAll(origin, boxSize, 0f, Vector2.right, adjustedDistance, punchable);
            hitDirection = 1;

        } else {

            //hit = Physics2D.CircleCastAll (origin, radius, -Vector2.right, distance, punchable);
            hit = Physics2D.BoxCastAll(origin, boxSize, 0f, -Vector2.right, adjustedDistance, punchable);
            hitDirection = 0;

        }
        //TODO  possibly apply a knockback to enemy.


        foreach (RaycastHit2D j in hit) {
            var collider = j.collider;

            if (collider != null) {

                // Attempt to handle the case where it hits an entity that can take damage.
                var takeDamage = (ITakeDamage)collider.GetComponent(typeof(ITakeDamage));

                if (takeDamage != null)
                    takeDamage.TakeDamage(damageToGive, gameObject);
                // Attempt to handle the case where a lever is hit.
                else if (collider.GetComponent<LeverBehavior>() != null)
                    j.collider.GetComponent<LeverBehavior>().ToggleDoor();

            }

        }
        yield return new WaitForSeconds(.18f);

        ongoingPunches--;
        if (ongoingPunches <= 0)
            playerAnim.SetBool("isPunching", false);


    }
}