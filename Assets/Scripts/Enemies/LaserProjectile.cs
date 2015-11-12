using UnityEngine;
using System.Collections;

// TODO implement into the new projectile class!!
public class LaserProjectile : MonoBehaviour
{

	float maxSpeed = 5f;
	public GameObject impactEffect;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;

		Vector3 velocity = new Vector3 (0, maxSpeed * Time.deltaTime, 0);

		pos += transform.rotation * velocity;

		transform.position = pos;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "MapGround" || other.tag == "Solid" || other.tag == "Destructable") {
			Instantiate (impactEffect, transform.position, transform.rotation);
			
			Destroy (gameObject);
			
		}
	}
}
