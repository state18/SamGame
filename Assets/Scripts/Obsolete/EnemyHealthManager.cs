using UnityEngine;
using System.Collections;

public class EnemyHealthManager : MonoBehaviour
{

	public int enemyHealth;				

	public GameObject deathEffect;		//Particle system from prefab. Will probably change for unique enemies TODO
	public bool killable;
	private SpriteRenderer enemySprite;

	//public float knockbackLength;


	//public float invulnerableLength;
	
	// Use this for initialization
	void Start ()
	{
		//rb = GetComponent<Rigidbody2D> ();
		//enemyPatrol = GetComponent<EnemyPatrol> ();
		enemySprite = GetComponent<SpriteRenderer> ();
		if (enemySprite == null) {
			enemySprite = GetComponentInChildren<SpriteRenderer> ();
		}
		//pc = FindObjectOfType<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (enemyHealth <= 0) {
			Instantiate (deathEffect, transform.position, transform.rotation);
			//adding points to ScoreManager could happen here
			Destroy (gameObject);
		}
	}

	public void giveDamage (int damageToGive)
	{
		if (killable) {
			enemyHealth -= damageToGive;
			Debug.Log ("damaged enemy");
			HurtEffect ();
		}
	}

	/*public void giveDamage (int damageToGive, Collider2D other) 
	{

		enemyHealth -= damageToGive;
		//rb.AddForce (new Vector2 (4f, 0f));
		if (other.transform.position.x < pc.transform.position.x) {
			enemyPatrol.knockFromRight = true;
			if (enemyPatrol.moveRight)
				enemyPatrol.knockbackCounter = knockbackLength;
			else {
				enemyPatrol.knockFromRight = false;
				if (!enemyPatrol.moveRight)
					enemyPatrol.knockbackCounter = knockbackLength;
			}
			Knockback ();

		}

	}*/
	private void HurtEffect ()
	{

		StartCoroutine ("HurtEffectCo");

	}
	public IEnumerator HurtEffectCo ()
	{


		InvokeRepeating ("EnemySpriteToggle", 0f, .1f);
		//Debug.Log ("blinking");
		
		yield return new WaitForSeconds (.6f);

		CancelInvoke ("EnemySpriteToggle");
		enemySprite.enabled = true;

	}

	private void EnemySpriteToggle ()
	{
		enemySprite.enabled = !enemySprite.enabled;
	}


		
		
}
	
	

