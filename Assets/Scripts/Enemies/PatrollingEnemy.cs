using System;
using UnityEngine;

public class PatrollingEnemy : Enemy {
   
    public enum PatrolType { IgnoreEdges, EdgeGuard}
    public PatrolType patrolType;
    public Transform edgeCheck;
    public LayerMask whatIsGround;

    private CharacterController2D controller;

    void Start() {
        Initialize();
        controller = GetComponent<CharacterController2D>();
        direction = Vector2.left;
        startPosition = transform.position;
        Health = MaxHealth;
    }

    void Update() {

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
}