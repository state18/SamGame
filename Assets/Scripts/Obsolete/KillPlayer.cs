using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour			//Obsolete now because of HurtPlayerOnContact script. This script kills the player instantly upon contact
{

	public LevelManager levelManager;

	// Use this for initialization
	void Start ()
	{
		levelManager = FindObjectOfType<LevelManager> ();  //get access to level manager to call the RespawnPlayer method later on
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.name == "Player") {				//if the player touches an entity with this script, they instantly die
			levelManager.RespawnPlayer ();
		}
	}
}
