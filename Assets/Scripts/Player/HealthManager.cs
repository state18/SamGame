using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour    //keeps track of the player health
{

	public int maxPlayerHealth;					//max possibility of player health
	public static int PlayerHealth;				//current player health

	
	public Toggle[] hearts;

	//Canvas mainCanvas;


	private LevelManager levelManager;
	private BuffManager buffManager;

	public bool isDead;							//is the player dead?

	// Use this for initialization
	void Start ()
	{
		//text = GetComponent<Text>();
		//mainCanvas = GetComponent<Canvas> ();
		//hearts = mainCanvas.GetComponentsInChildren<Toggle> ();
		foreach (Toggle t in hearts) {
			t.isOn = true;
		} 
		//Debug.Log (hearts.Length);
		PlayerHealth = maxPlayerHealth;
		levelManager = FindObjectOfType<LevelManager> ();
		buffManager = FindObjectOfType<BuffManager> ();
		isDead = false;

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (PlayerHealth <= 0 && !isDead) {			//runs if the player isn't already dead and their hp is at 0 or below. the player is respawned by the level manager
			PlayerHealth = 0;
			levelManager.RespawnPlayer ();
			isDead = true;
		}

		//text.text = "" + PlayerHealth;


	}

	public void HurtPlayer (int damageToGive)	// This is often invoked by any "harm player" scripts.
	{
		if (!BuffManager.isInvulnerable) {
			for (int i = PlayerHealth - 1; i > PlayerHealth - damageToGive - 1; i--) {
				if (i < 0 || hearts [i] == null)
					break;
				hearts [i].isOn = false;

			}
			PlayerHealth -= damageToGive;
			GetComponent<AudioSource> ().Play ();

			if (PlayerHealth > 0)
				buffManager.BuffInvulnerability ();
		}

		
	}

	public void FullHealth ()					//this restores the player's life to max hp
	{
		PlayerHealth = maxPlayerHealth;

		foreach (Toggle t in hearts) {
			t.isOn = true;
		} 
	}
}
