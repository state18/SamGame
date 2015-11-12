using UnityEngine;
using System.Collections;

public class LadderTop : MonoBehaviour
{
	//Transform otherT;
	PlayerController pc;

	Animator anim;

	public float yOffset;

	// Use this for initialization
	void Start ()
	{
		pc = FindObjectOfType<PlayerController> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "PlayerTrigger") {

			//pc = other.GetComponentInParent<PlayerController> ();
			//otherT = other.GetComponentInParent<Transform> ();

			if (!pc.isClimbing && Input.GetAxisRaw ("Vertical") == -1) {

				other.transform.parent.position = new Vector3 (transform.position.x, other.transform.parent.position.y, other.transform.parent.position.z);
				other.transform.parent.Translate (0f, yOffset, 0f);

				pc.CanClimb = true;
				pc.isClimbing = true;

			} else if (pc.isClimbing && Input.GetAxisRaw ("Vertical") == 1) {

				anim = other.GetComponentInParent<Animator> ();
				anim.SetTrigger ("LadderExit");
				other.transform.parent.position = new Vector3 (other.transform.position.x, other.transform.parent.position.y, other.transform.parent.position.z);
				other.transform.parent.Translate (0f, -yOffset, 0f);


				pc.CanClimb = false;
				pc.isClimbing = false;


			}
		}

	}
}
