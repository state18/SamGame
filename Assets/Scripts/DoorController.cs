using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{

	public bool isOpen;

	SpriteRenderer[] srArray;

	// Use this for initialization
	void Start ()
	{

		isOpen = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	
	}

	public void OpenDoor ()
	{
		srArray = GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer sr in srArray) {

			if (sr.tag == "DoorRemove") {
				sr.enabled = false;
			} else if (sr.enabled) {
				sr.enabled = false;
			} else {
				sr.enabled = true;
			}
			

		}
		GetComponent<BoxCollider2D> ().enabled = false;
		isOpen = true;
	}

	public void CloseDoor ()
	{
		srArray = GetComponentsInChildren<SpriteRenderer> ();
		
		foreach (SpriteRenderer sr in srArray) {
			
			if (sr.tag == "DoorRemove") {
				sr.enabled = true;
			} else if (!sr.enabled) {
				sr.enabled = true;
			} else {
				sr.enabled = false;
			}
			
			
		}
		GetComponent<BoxCollider2D> ().enabled = true;
		isOpen = false;
	}
}
