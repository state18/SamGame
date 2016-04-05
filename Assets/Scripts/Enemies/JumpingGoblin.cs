using UnityEngine;
using System.Collections;

/// <summary>
/// This tall enemy jumps around constantly.
/// </summary>
public class JumpingGoblin : Enemy {

    private CharacterController2D controller;

    // Use this for initialization
    void Start() {
        Initialize();
        controller = GetComponent<CharacterController2D>();
        direction = Vector2.right;
        startPosition = transform.position;
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update() {
        if (!IsDead) {
            // Applies velocity
            controller.SetHorizontalForce(direction.x * Speed);

            // Turn around if colliding with a wall
            if ((direction.x < 0 && controller.State.IsCollidingLeft) || (direction.x > 0 && controller.State.IsCollidingRight))
                Flip();

            controller.BasicJump();
        }
    }
}
