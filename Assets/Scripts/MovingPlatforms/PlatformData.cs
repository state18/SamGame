using UnityEngine;
using System.Collections;

public class PlatformData : MonoBehaviour
{

	public bool playerFreed;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate ()
	{

	}

	void LateFixedUpdate ()
	{

	}

	public Vector2 GetVelocity ()
	{

		if (playerFreed) {
			return Vector2.zero;
		} else
			return GetComponent<Rigidbody2D> ().velocity;
		
	}

	public void FreePlayer (bool isFreed)
	{
		playerFreed = isFreed;
	}




}