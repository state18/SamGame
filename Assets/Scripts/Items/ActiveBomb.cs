using UnityEngine;
using System.Collections;

public class ActiveBomb : MonoBehaviour
{
	public int damage;
	float lifetime = 3f;
	public float blastRadius;
	Animator bombAnimator;
	public GameObject detonationParticle;
	
	// Use this for initialization
	void Start ()
	{
		bombAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		lifetime -= Time.deltaTime;
		
		if (lifetime < 0) {
			Instantiate (detonationParticle, transform.position, transform.rotation);
            // Every Collider2D within the blast radius will be captured in this array
			Collider2D[] bombHit = Physics2D.OverlapCircleAll (new Vector2 (transform.position.x, transform.position.y), blastRadius);
			
			foreach (Collider2D n in bombHit) {

                var takeDamage = (ITakeDamage)n.GetComponent(typeof(ITakeDamage));
                if (takeDamage != null)
                    takeDamage.TakeDamage(damage, gameObject);
                else{
                    var destructable = n.GetComponent<Destructable>();
                    if (destructable != null)
                        destructable.Destroy();
                }
				// TODO Handle scenario for destructables and possibly lever toggling!
			}
			Destroy (gameObject);
		} else if (lifetime < 1) {
			bombAnimator.SetTrigger ("ToThird");
		} else if (lifetime < 2) {
			bombAnimator.SetTrigger ("ToSecond");
		}
	}

}
