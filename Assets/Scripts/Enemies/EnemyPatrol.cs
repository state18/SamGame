using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour    //This is for enemies that walk back and forth along the ground. They turn around at walls and edges.
{

	public float moveSpeed;
	public bool moveRight;

	bool hittingWall = false;
	public Transform wallCheck;
	public float wallCheckRadius = 0.08f;
	public LayerMask whatIsWall;

	private bool notAtEdge;
	public bool knockFromRight;
	public float knockback;						//how hard is the knockback?
	public float knockbackLength;				//how long is the knockback? (sent to hurt player script)
	public float knockbackCounter;   
	public Transform edgeCheck;
	private Rigidbody2D rbEnemy;
	//private bool inEnemy;

	// Use this for initialization
	void Start ()
	{
		rbEnemy = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate ()
	{
		hittingWall = Physics2D.OverlapCircle (wallCheck.position, wallCheckRadius, whatIsWall);			//checks for walls/edges

		notAtEdge = Physics2D.OverlapCircle (edgeCheck.position, wallCheckRadius, whatIsWall);

		if (hittingWall || !notAtEdge)
			Flip ();																			//Flips the entity if at wall or edge


		if (moveRight) {

			rbEnemy.velocity = new Vector2 (moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		} else {

			rbEnemy.velocity = new Vector2 (-moveSpeed, GetComponent<Rigidbody2D> ().velocity.y);
		}
		

			
			
		
	}

	void OnTriggerEnter2D (Collider2D other)				//Flips if the entity collides with another enemy FIXME maybe change to overlap circle or something for better collisions
	{
		if (other.tag == "Enemy")
			Flip ();

	}


	void Flip ()
	{
		moveRight = !moveRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		knockbackCounter = 0f;
	}




}
