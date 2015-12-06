using System;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour, ITakeDamage, IRespawnable {
    public float Speed;
    public float FireRate = 1;
    public Projectile Projectile;
    public GameObject DestroyedEffect;
    public int PointsToGivePlayer;
    public float MaxHealth;
    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    public enum PatrolType { IgnoreEdges, EdgeGuard}
    public PatrolType patrolType;
    public Transform edgeCheck;
    public LayerMask whatIsGround;

    private CharacterController2D controller;
    private Vector2 direction;
    // The starting position of the enemy is remembered for respawning purposes.
    private Vector2 startPosition;

    public void Start() {
        controller = GetComponent<CharacterController2D>();
        direction = new Vector2(-1, 0);
        startPosition = transform.position;
        Health = MaxHealth;
    }

    public void Update() {

        if (!IsDead) {
            // Applies velocity
            controller.SetHorizontalForce(direction.x * Speed);

            // Turn around if colliding with a wall
            if ((direction.x < 0 && controller.State.IsCollidingLeft) || (direction.x > 0 && controller.State.IsCollidingRight)) 
                Flip();
            

            // If EdgeGuard is on, check for an edge. If next to an edge, turn around.
            if (patrolType == PatrolType.EdgeGuard && controller.State.IsCollidingBelow) {
                if (edgeCheck == null)
                    Debug.LogError("EdgeCheck coordinate is null!");
                else if (!Physics2D.OverlapCircle(edgeCheck.position, .1f, whatIsGround))
                    Flip();
                        

            }
                
        }
    }
    public void TakeDamage(int damage, GameObject instigator) {
        // Possibly add points here, if killed by the player, possibly through a separate score manager
        Health -= damage;
        if (Health <= 0) 
            KillMe();
    }

    public void KillMe() {
        IsDead = true;
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        LevelManagerProto.Instance.AddDeadEnemy(gameObject);
        gameObject.SetActive(false);
    }

    public void RespawnMe() {
        transform.position = startPosition;
        IsDead = false;
        Health = MaxHealth;
    }

    /// <summary>
    /// Changes the direction of the entity and adjusts the scale accordingly by negating the x component.
    /// </summary>
    public void Flip() {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    
}