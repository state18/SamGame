using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlatformBasic : MonoBehaviour
{
	Rigidbody2D rb;

	public float targetedVelocity;
	public float maxDistanceToGoal = .1f;
	public int startingPoint;

	public MovingPath path;


	private IEnumerator<Transform> currentPoint;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();

		if (path == null) {
			Debug.LogError ("Path cannot be null", gameObject);
			return;
		}
		
		currentPoint = path.GetPathEnumerator ();
		for (int i = 0; i <= startingPoint; i++)
			currentPoint.MoveNext ();

		
		if (currentPoint.Current == null)
			return;
		
		transform.position = currentPoint.Current.position;


	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate ()
	{
		if (currentPoint == null || currentPoint.Current == null)
			return;
		
		var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
		if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {
			currentPoint.MoveNext ();
			ChangeDirection (new Vector2 (currentPoint.Current.transform.position.x, currentPoint.Current.transform.position.y));
		}
	}

	public void ChangeDirection (Vector2 destinationNode)
	{


		Vector2 distanceVector = destinationNode - new Vector2 (transform.position.x, transform.position.y);
		float magnitude = distanceVector.magnitude / targetedVelocity;

		
		rb.velocity = new Vector2 (distanceVector.x / magnitude, distanceVector.y / magnitude);
		//Debug.Log ("velocity " + rb.velocity);
	}
}
