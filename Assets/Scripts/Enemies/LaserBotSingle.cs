using UnityEngine;
using System.Collections;

public class LaserBotSingle : MonoBehaviour
{
	public float rotationSpeed;
	public float shotDelay;
	public float detectionRange;

	public LayerMask player;

	bool playerInRange;
	float shotDelayCounter;

	Transform playerPosition;
	Quaternion defaultRotation;
	AudioSource fireSound;

	public Transform firePoint;
	public GameObject laserPrefab;

	// Use this for initialization
	void Start ()
	{
		playerPosition = GameObject.FindGameObjectWithTag ("Player").transform;
		defaultRotation = transform.rotation;
		fireSound = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (shotDelayCounter > 0) {
			shotDelayCounter -= Time.deltaTime;
		}
		playerInRange = Physics2D.OverlapCircle (transform.position, detectionRange, player);
		//Debug.Log ("Player in range" + playerInRange);

		if (playerInRange) { 

			RotateToFace (playerPosition.position);

			if (shotDelayCounter <= 0)
				Shoot ();

		} else if (transform.rotation != defaultRotation) {

			RotateToDefault (defaultRotation);
		}


	}

	void RotateToFace (Vector3 targetPosition)
	{
		Vector3 dir = targetPosition - transform.position;
		dir.Normalize ();
		
		float zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - 90;
		
		Quaternion desiredRot = Quaternion.Euler (0, 0, zAngle);
		
		transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRot, rotationSpeed * Time.deltaTime);
	}

	void RotateToDefault (Quaternion targetPosition)
	{
		transform.rotation = Quaternion.RotateTowards (transform.rotation, defaultRotation, rotationSpeed * Time.deltaTime);
	}

	void Shoot ()
	{

		Instantiate (laserPrefab, firePoint.position, transform.rotation);
		fireSound.Play ();
		shotDelayCounter = shotDelay;
	}






}