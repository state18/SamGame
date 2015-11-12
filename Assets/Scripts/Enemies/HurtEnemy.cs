using UnityEngine;
using System.Collections;

public class HurtEnemy : MonoBehaviour		//This script will damage enemies on contact
{
	public int damageToGive;				//How much damage will the enemy take?

	public float bounceOnenemy;	//TODO        for possible future of jumping on enemies to do damage


	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Enemy") {													//If an object with this script attached hits an enemy, they take damage
			other.GetComponent<EnemyHealthManager> ().giveDamage (damageToGive);

		}
	}
}
