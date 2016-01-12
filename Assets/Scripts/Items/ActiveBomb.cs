using UnityEngine;
using System.Collections;

public class ActiveBomb : MonoBehaviour {
    public int damage;
    float lifetime = 3f;
    public float blastRadius;
    public float SpeedAcceleration;
    Animator bombAnimator;
    public GameObject detonationParticle;
    private CharacterController2D controller;

    // Use this for initialization
    void Start() {
        bombAnimator = GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update() {
        lifetime -= Time.deltaTime;

        // This is for consistency when interacting with platforms the same way the player does. The player's force is set each frame based on input and current speed, and then platforms are taken into consideration.
        // For the bomb, no input is given and therefore the speed should be lerped between current speed and 0 with step distance being equal to the acceleration * time between frames.
        controller.SetHorizontalForce(Mathf.Lerp(controller.Velocity.x, 0f, SpeedAcceleration * Time.deltaTime));

        if (lifetime < 0) {
            Instantiate(detonationParticle, transform.position, transform.rotation);
            // Every Collider2D within the blast radius will be captured in this array
            Collider2D[] bombHit = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), blastRadius);

            foreach (Collider2D n in bombHit) {
                // Give damage to objects that take damage.
                var takeDamage = (ITakeDamage)n.GetComponent(typeof(ITakeDamage));
                if (takeDamage != null)
                    takeDamage.TakeDamage(damage, gameObject);

                // Destroy objects that can be destroyed.
                var destructable = n.GetComponent<Destructable>();
                if (destructable != null)
                    destructable.Destroy();

                // Toggle levers if object is a lever.
                var leverInteraction = n.GetComponent<LeverBehavior>();
                if (leverInteraction != null)
                    leverInteraction.ToggleDoor();

                // TODO Handle scenario for destructables and possibly lever toggling!
            }
            Destroy(gameObject);
        } else if (lifetime < 1) {
            bombAnimator.SetTrigger("ToThird");
        } else if (lifetime < 2) {
            bombAnimator.SetTrigger("ToSecond");
        }
    }

}
