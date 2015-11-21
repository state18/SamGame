using UnityEngine;
using System.Collections;

public class BombItem : Item
{
	public Transform bombSpawn;
	public GameObject bombPrefab;
	//private float cooldown = 0f;
	public float timeUntilDetonation;
	private int ongoingBombs;         //no more than 2 at a time
	
	public BombItem ()
	{
		InHand = false;
		IsObtained = false;
		ongoingBombs = 0;
	}
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		//if (cooldown > 0)
		//	cooldown -= Time.deltaTime;
		// TODO handle climbing later.
		if (InHand && Input.GetKeyDown (KeyCode.X)) {
			Use ();
		}
	}
	
	public  void Use ()
	{
		if (ongoingBombs < 2) {

			StartCoroutine ("UseCo");
		}
	}

	public IEnumerator UseCo ()
	{
		Debug.Log ("Bomb Dropped");
		Instantiate (bombPrefab, bombSpawn.position, bombSpawn.rotation);
		//cooldown = cooldownTime;
		ongoingBombs++;
		yield return new WaitForSeconds (timeUntilDetonation);
		ongoingBombs--;
		//if (ongoingBombs <= 0)

	}
}
