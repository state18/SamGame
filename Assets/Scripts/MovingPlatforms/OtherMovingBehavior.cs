using UnityEngine;
using System.Collections;

public class OtherMovingBehavior : MonoBehaviour			//attach this script to an object/enemy that moves along moving platforms/ conveyer belts as the player would.
{        
	public Transform movingCheck;
	public float movingRadius;
	public float speed;
	public Vector2 boxCastSize;

	public LayerMask whatIsMovingPlatform;
	public LayerMask whatIsConveyer;

	PlatformData currentPlatform;
	ConveyorPlatform currentConveyor; 
	Rigidbody2D rb;
	//EnemyPatrol ep;
	//Vector2 startVelocity;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
		//	ep = GetComponent<EnemyPatrol> ();


	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void FixedUpdate ()
	{
		RaycastHit2D platformHit = Physics2D.BoxCast (movingCheck.position, boxCastSize, 0f, Vector2.down, movingRadius, whatIsMovingPlatform);
		RaycastHit2D conveyerHit = Physics2D.BoxCast (movingCheck.position, boxCastSize, 0f, Vector2.down, movingRadius, whatIsConveyer);

		if (platformHit) {           							 //Are we on a moving platform? If so, obtain its data and use it below
			//Debug.Log ("On platform");
			currentPlatform = platformHit.transform.GetComponent<PlatformData> ();
		} else {
			currentPlatform = null;
		}
		
		if (conveyerHit) {			 							 // Are we on a conveyer belt? If so, obtain its data and use it below
			//Debug.Log ("On conveyer belt");
			currentConveyor = conveyerHit.transform.GetComponent<ConveyorPlatform> ();
		} else {
			currentConveyor = null;
		}

		if (currentPlatform != null && !currentPlatform.playerFreed) {		//are we on a moving platform?
			//Debug.Log ("on moving platform");
			var platformVelocity = currentPlatform.GetVelocity ();
			//Debug.Log (platformVelocity);
			rb.velocity = new Vector2 (rb.velocity.x, 0f);				//important: setting y component of velocity to 0 prevents weird bouncing behavior on platforms
			rb.velocity += platformVelocity;
			
			//rb.gravityScale = 0;
		} else if (currentConveyor != null) {
			//Debug.Log ("on conveyer belt");
			var conveyerVelocity = currentConveyor.GetVelocity ();
			//Debug.Log (conveyerVelocity);
			rb.velocity += new Vector2 (conveyerVelocity, 0f);
		}
	}
}
