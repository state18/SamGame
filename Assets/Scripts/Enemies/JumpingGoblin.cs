using UnityEngine;
using System.Collections;

public class JumpingGoblin : Enemy {

    private CharacterController2D controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController2D>();
        direction = Vector2.left;
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

            if(controller.CanJump)
                controller.Jump();
        }
    }
}
