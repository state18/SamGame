using UnityEngine;
using System.Collections;

[System.Obsolete]
public class BulletController : MonoBehaviour
{

	public float speed;					//how fast does the bullet travel?

	public Player player;

	//TODO make particle effect here

	public GameObject impactEffect;			//particle effect when bullet hits something
	//public GameObject enemyDeathEffect;

	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<Player> ();

		if (player.transform.localScale.x < 0) {			//direction of shot changes with player's facing direction
			speed = -speed;
		}

		//Rigidbody2D rb = GetComponent <Rigidbody2D> ();
		//rb.velocity = new Vector2 (speed, rb.velocity.y);
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		
		Vector3 velocity = new Vector3 (speed * Time.deltaTime, 0, 0);
		
		pos += transform.rotation * velocity;
		
		transform.position = pos;

	}

	void OnTriggerEnter2D (Collider2D other)
	{
        
        // TODO Handle damaging enemies and interacting with other terrain. (same way as the sword and bomb most likely)

		if (other.tag == "MapGround" || other.tag == "Enemy" || other.tag == "Solid" || other.tag == "Destructable") {
			Instantiate (impactEffect, transform.position, transform.rotation);

			Destroy (gameObject);

		}

	}
}
