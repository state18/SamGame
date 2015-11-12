using UnityEngine;
using System.Collections;

public class LevelManager: MonoBehaviour
{
	public GameObject currentCheckpoint;

	private PlayerController player;
	private Animator anim;

	private Collider2D[] playerColliders;

	public GameObject deathParticle;
	public GameObject respawnParticle;

	public float respawnDelay;

	private CameraController cam;

	//private float gravityStore;

	public HealthManager healthManager;
	public BuffManager buffManager;

	bool hasRunThisFrame = false;     //for RespawnPlayer Bug



	void Awake ()
	{

	}


	// Use this for initialization
	void Start ()							//references to other scripts are made here
	{

		player = FindObjectOfType<PlayerController> ();



		anim = player.GetComponent<Animator> ();

		cam = FindObjectOfType<CameraController> ();

		healthManager = FindObjectOfType<HealthManager> ();

		buffManager = FindObjectOfType<BuffManager> ();

		currentCheckpoint = GameObject.FindGameObjectWithTag ("StartingPoint");

		//Camera.main.orthographicSize = Screen.height / (2f * 16);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void RespawnPlayer ()		//called upon player death
	{
		StartCoroutine ("RespawnPlayerCo");
	}

	public IEnumerator RespawnPlayerCo ()		//coroutine
	{

		if (!hasRunThisFrame) {								//steps taken to deactivate the player are done here

			hasRunThisFrame = true;
			Instantiate (deathParticle, player.transform.position, player.transform.rotation);
			player.isClimbing = false;
			player.CanClimb = false;
			//player.gameObject.SetActive (false);
			BuffManager.isInvulnerable = false;
			player.GetComponent<Renderer> ().enabled = false;
			player.GetComponent<Rigidbody2D> ().isKinematic = true;

			playerColliders = player.GetComponents <Collider2D> ();
			for (int i = 0; i < playerColliders.Length; i++) {
				playerColliders [i].enabled = false;
			}
			cam.IsFollowing = false;

			ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
			yield return new WaitForSeconds (respawnDelay);
			yield return StartCoroutine (sf.FadeToBlack ());

			//Player Respawns here. The previous code is pretty much undone after a delay.
			//player.gameObject.SetActive (true);
			player.transform.position = currentCheckpoint.transform.position;
			//player.enabled = true;
			player.knockbackCounter = 0f;

			player.GetComponent<Rigidbody2D> ().isKinematic = false;
			anim.speed = 1f;
			healthManager.FullHealth ();
			healthManager.isDead = false;
			for (int i = 0; i < playerColliders.Length; i++) {
				playerColliders [i].enabled = true;
			}
			player.GetComponent<Renderer> ().enabled = true;
			cam.transform.position = new Vector3 (currentCheckpoint.transform.position.x, currentCheckpoint.transform.position.y, -10);

			yield return StartCoroutine (sf.FadeToClear ());
			cam.IsFollowing = true;
			//Instantiate (respawnParticle, player.transform.position, player.transform.rotation);

			hasRunThisFrame = false;
		}
	}


	
}
