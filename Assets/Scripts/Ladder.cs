using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{

	private Player player;

	//TODO snap player to center of ladder transform

	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<Player> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.tag == "PlayerTrigger") {

			//var pc = other.GetComponentInParent<PlayerController> (); 
			//player.CanClimb = true;

			
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{

		if (other.tag == "PlayerTrigger") {
			//var pc = other.GetComponentInParent<PlayerController> ();
			//player.CanClimb = false;
			//player.isClimbing = false;
		}

	}
}
