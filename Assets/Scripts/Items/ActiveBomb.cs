using UnityEngine;
using System.Collections;

public class ActiveBomb : MonoBehaviour
{
	public int damage;
	float lifetime = 3f;
	public float blastRadius;
	Animator bombAnimator;
	Rigidbody2D rb;
	public LayerMask destructable;
	public GameObject detonationParticle;
	
	// Use this for initialization
	void Start ()
	{
		bombAnimator = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		lifetime -= Time.deltaTime;
		
		if (lifetime < 0) {
			Instantiate (detonationParticle, transform.position, transform.rotation);
			Collider2D[] bombHit = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x, transform.position.y), blastRadius, destructable);
			
			foreach (Collider2D n in bombHit) {
				if (n.tag == "Player") {
					n.GetComponent<HealthManager> ().HurtPlayer (damage);
				} else if (n.tag == "Enemy") {
					n.GetComponent<EnemyHealthManager> ().giveDamage (damage);
				} else if (n.tag == "Destructable") {
					n.GetComponent<Destructable> ().Explode ();
				} else if (n.tag == "Lever") {
					var leverControl = n.GetComponent<LeverBehavior> ();
					if (!leverControl.isOn) {
						leverControl.TellDoorOpen ();
					} else if (leverControl.isOn)
						leverControl.TellDoorClose ();
				}
			}
			Destroy (gameObject);
		} else if (lifetime < 1) {
			bombAnimator.SetTrigger ("ToThird");
		} else if (lifetime < 2) {
			bombAnimator.SetTrigger ("ToSecond");
		}
	}

	void FixedUpdate ()
	{
		rb.velocity = new Vector2 (0f, rb.velocity.y);

	}
}
