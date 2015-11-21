using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FlyingBot : MonoBehaviour
{
	//Patrol editable fields

	public PathDefinition patrolPath;       //Where will this entity patrol?
	public int startingPoint;           //From which node will the entity begin patrolling?
	public float targetedVelocity;		//How fast does the entity move by default?
	public float detectionRadius;		//How far away can the entity see the player?
	public LayerMask whatIsPlayer;	
	public Rect botBounds;				//How far away can the entity follow the player?
	
	//MoveToPlayer editable fields
	
	public float yOffsetFromPlayer;					//distance between the player and the target point of the entity (entity targets a point around player, not the player directly)
	public float delayUntilSlam;					//how long will the entity wait until it slams after reaching the player?
	//MoveToPlayer coroutine flag check
	bool isPreSlamDownwardsRunning = false;			//The coroutine should not be executed repeatedly while it is already running

	//SlamDownwards editable fields
	public LayerMask whatisGround;
	
	StateMachine brain;
	
	Transform playerTransform;
	Rigidbody2D playerRigidbody2D;
	
	
	private IEnumerator<Transform> currentPoint;
	
	// Use this for initialization
	void Start ()
	{
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		playerRigidbody2D = playerTransform.GetComponent<Rigidbody2D> ();
		
		//AI initialized, and Patrol state is set
		brain = new StateMachine (PatrolState);

		//brain.pushState (Patrol);
		
		
		if (patrolPath == null) {
			Debug.LogError ("Path cannot be null", gameObject);
			return;
		}
		
		currentPoint = patrolPath.GetPathEnumerator ();
		for (int i = 0; i <= startingPoint; i++)
			currentPoint.MoveNext ();
		
		
		if (currentPoint.Current == null)
			return;
		
		transform.position = currentPoint.Current.position;
	}
	
	void Update ()
	{
		// AI instructions received and executed
		//brain.ManualUpdate ();
		
		//Physics adjustments made TODO
		
		//Debug.Log ("brain updated");
	}
	
	public IEnumerator PatrolState ()	//Move around a contained area
	{
		Debug.Log ("Patrolling");

		#region Patrol Entry
		//TODO maybe?
		#endregion

		while (true) {


			//Enemy moves towards the next point in its path. If at destination, cycle to the next point
			#region Patrol Action
		
			float maxDistanceToGoal = .1f;
		
			if (currentPoint == null || currentPoint.Current == null)
				yield return null;
			var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
			if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {								//Has the entity arrived at the destination node?
			
				currentPoint.MoveNext ();
			
			}
			transform.position = Vector2.MoveTowards (transform.position, currentPoint.Current.position, Time.deltaTime * targetedVelocity);
			#endregion

			//Transition Checks: MoveToPlayer
			#region To MoveToPlayer
			if (Physics2D.OverlapCircle (transform.position, detectionRadius, whatIsPlayer) && botBounds.Contains (playerTransform.position)) { //TODO CircleRaycast so it doesn't see player through walls, also  don't call this every frame, use coroutine
				brain.popState ();
				brain.pushState (MoveToPlayerState);
			}
			#endregion 
		
			yield return null;
		}
	}
	
	public IEnumerator MoveToPlayerState ()	//Fly over the player while they are in range of the entity AND within the entity's max bounds
	{
		Debug.Log ("Moving To Player");

		#region MoveToPlayer Entry
		//TODO maybe?
		#endregion

		while (true) {
		
			#region MoveToPlayer Action
		
			float maxDistanceToGoal = .1f;
			float step = playerRigidbody2D.velocity.magnitude * 1.5f;			//the entity will move a little bit faster than the player is moving
		
			if (step < 1)												//if the player isn't moving, move at targeted velocity
				step = targetedVelocity;
		
			Vector3 actualDestination = new Vector3 (0, yOffsetFromPlayer, 0) + playerTransform.position;		//the point where the entity wants to go
			transform.position = Vector2.MoveTowards (transform.position, actualDestination, step * Time.deltaTime);
			#endregion
		
			//Transition Checks: SlamDownwards, Patrol
		
			#region To SlamDownwards
			var distanceSquared = (transform.position - actualDestination).sqrMagnitude;
		
			if (!isPreSlamDownwardsRunning && distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {					//entity has reached the targeted location 
				Debug.Log ("arrived at target destination");
				isPreSlamDownwardsRunning = true;
				StartCoroutine ("PreSlamDownwardsCoroutine");				//begins a timer that switches to slam state when it expires, but only if player is still being followed!
			
				//brain.popState ();
				//brain.pushState (SlamDownwardsState);
			
			}
		
		
			#endregion
		
			#region To Patrol
		
			if (!botBounds.Contains (transform.position)) {
				Debug.Log ("movetoplayer to patrolling");
				StopCoroutine ("PreSlamDownwardsCoroutine");
				isPreSlamDownwardsRunning = false;
				brain.popState ();
				brain.pushState (PatrolState);
			}
		
			#endregion

			yield return null;
		}
	}
	
	
	public IEnumerator SlamDownwardsState ()	// After a brief delay, slam downwards (this is the window for the player to kill this enemy)
	{
		#region SlamDownwards Entry
		//TODO Speed up the animation of the bot, and give its color a different tint to let the player know it is about to slam.
		//After a very short delay, the bot will descend a certain length or until it touches the ground. It will remain on the ground for a short while.
		//This is where the player will be able to attack. Afterwards, transition it back to Patrol State.
		Animator botAnim = GetComponent<Animator> ();														//The bot changes to a red tint and doubles its animation speed
		SpriteRenderer botSprite = GetComponent<SpriteRenderer> ();
		botAnim.speed = 2;
		botSprite.color = Color.red;
		RaycastHit2D hit = Physics2D.Raycast (new Vector2 (transform.position.x, transform.position.y), Vector2.down, 1f, whatisGround);		//stores info about the ground hit by raycasting
		float distanceOffset = GetComponent<BoxCollider2D> ().size.y / 2;																		//this offsets the ray to the bottom of the bot
		float distanceToSlam = hit.distance - distanceOffset;
		Debug.Log ("Distance to slam: " + distanceToSlam);
		#endregion
		while (true) {

			#region SlamDownwards Action
			
			#endregion

			#region ToPatrol

			#endregion
			yield return null;
		}
	}
	
	IEnumerator PreSlamDownwardsCoroutine ()					//facilitates the transition from MovingToPlayer to SlamDownwards (will not always complete!)
	{															//This is not a real "state" recognized by the state machine, but merely a transition check that lasts over time
		yield return StartCoroutine (WaitCo.Wait (delayUntilSlam));		//Continue following the player but get ready to slam!
		Debug.Log ("done waiting");
		if (brain.getCurrentState () != PatrolState) {			//If entity is patrolling now, then there's no need to slam. 
			Debug.Log ("performing slam");		
			isPreSlamDownwardsRunning = false;
			brain.popState ();									//protocol for switching state to SlamDownwards
			brain.pushState (SlamDownwardsState);
			
		}
	}
	
	
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (botBounds.max, botBounds.min);
	}
	
	
}
