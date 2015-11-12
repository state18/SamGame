using UnityEngine;
using System.Collections;

public class BuffManager : MonoBehaviour  //handles status effects the player may endure
{

	public static bool isInvulnerable;		//can the player be damaged?
	public  float invulnerableLength;		//how long will invulnerability last?



	SpriteRenderer playerSprite;			//holds the player's sprite


	// Use this for initialization

	void Start ()
	{
		isInvulnerable = false;				//the player will never start out invulnerable

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public  void BuffInvulnerability ()
	{
		StartCoroutine ("BuffInvulnerabilityCo");		//calls coroutine below (because there is a yield statement)
	}

	IEnumerator BuffInvulnerabilityCo ()    		   	//Player cannot be damaged while this is active. Sprite will toggle on/off as an indicator
	{
		isInvulnerable = true;
		InvokeRepeating ("SpriteToggle", 0f, .1f);

		yield return new WaitForSeconds (invulnerableLength);
		isInvulnerable = false;
		CancelInvoke ("SpriteToggle");
		playerSprite.enabled = true;


	}

	public void SpriteToggle ()							//Flickers the player sprite on/off each time it is called
	{
		if (isInvulnerable) {
			playerSprite = GetComponent<SpriteRenderer> ();

			playerSprite.enabled = !playerSprite.enabled;
		}
	}
}
