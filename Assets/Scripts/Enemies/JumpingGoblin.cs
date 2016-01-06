using UnityEngine;
using System.Collections;

public class JumpingGoblin : MonoBehaviour, ITakeDamage, IRespawnable {

    public float Speed;
    public GameObject DestroyedEffect;
    public float MaxHealth;
    public float jumpDelay;
    public float Health { get; private set; }
    public bool IsDead { get; private set; }

    private CharacterController2D controller;
    private Vector2 direction;
    private Vector2 startPosition;
    private float jumpTimer;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController2D>();
        direction = Vector2.left;
        startPosition = transform.position;
        Health = MaxHealth;
        jumpTimer = jumpDelay;
	}

    // Update is called once per frame
    void Update() {
        if (!IsDead) {
            // Applies velocity
            controller.SetHorizontalForce(direction.x * Speed);

            // Turn around if colliding with a wall
            if ((direction.x < 0 && controller.State.IsCollidingLeft) || (direction.x > 0 && controller.State.IsCollidingRight))
                Flip();

            if(jumpTimer <= 0 && controller.CanJump) {
                controller.Jump();
                jumpTimer = Random.Range(0, 2) + jumpDelay;
            }

            jumpTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Changes the direction of the entity and adjusts the scale accordingly by negating the x component.
    /// </summary>
    public void Flip() {
        direction = -direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void KillMe() {
        IsDead = true;
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        LevelManager.Instance.AddDeadEnemy(gameObject);
        gameObject.SetActive(false);
    }
    public void RespawnMe() {
        transform.position = startPosition;
        IsDead = false;
        Health = MaxHealth;
    }

    public void TakeDamage(int damage, GameObject instigator) {
        Health -= damage;
        if (Health <= 0)
            KillMe();
    }
}
