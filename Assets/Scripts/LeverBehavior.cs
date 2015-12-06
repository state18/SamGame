using UnityEngine;
using System.Collections;

// TODO rework the on/ off methods into a single toggle method.
public class LeverBehavior : MonoBehaviour
{
	public DoorController door;
	public bool isOn;
	bool playerInside;
	Animator anim;

	// Use this for initialization
	void Start ()
	{
		isOn = false;
		playerInside = false;
		anim = GetComponent<Animator> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerInside && !isOn && Input.GetButtonDown ("Up")) {

			TellDoorOpen ();

		} else if

		 (playerInside && isOn && Input.GetButtonDown ("Up")) {

			TellDoorClose ();
		}
	}

	public void TellDoorOpen ()
	{
		if (door == null) {
			Debug.Log ("no door attached");
			return;
		}
		isOn = true;
		anim.SetBool ("On", true);
		door.OpenDoor ();
		Debug.Log ("Door Opened");
		GetComponent<AudioSource> ().Play ();
	}

	public void TellDoorClose ()
	{
		if (door == null) {
			Debug.Log ("no door attached");
			return;
		}
		isOn = false;
		anim.SetBool ("On", false);
		door.CloseDoor ();
		Debug.Log ("Door closed");
		GetComponent<AudioSource> ().Play ();
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			playerInside = true;
		} else if (other.tag == "Projectile" && !isOn) {
			TellDoorOpen ();
		} else if (other.tag == "Projectile" && isOn) {
			TellDoorClose ();
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Player") {
			playerInside = false;
		} 
	}
}
