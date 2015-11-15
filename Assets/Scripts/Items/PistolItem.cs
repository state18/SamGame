using UnityEngine;
using System.Collections;
/// <summary>
/// Controls how the pistol item works. For information on the bullets that are fired from the pistol,
/// check out the BulletController class.
/// </summary>
public class PistolItem : Item
{
    // TODO Use new projectile system for the bullets!
	public Transform firePoint;
	public GameObject bulletPrefab;	
	private Animator playerAnim;
	private Player player;

	private int ongoingShots;
	private float cooldown = 0f;
	public float cooldownTime;

	public PistolItem ()
	{
		InHand = false;
		IsObtained = false;
		ongoingShots = 0;
	}
	// Use this for initialization
	void Start ()
	{
        player = FindObjectOfType<Player>();
        playerAnim = player.GetComponent<Animator>();
        
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
        // TODO Handle interaction with climbing.
		if (InHand && Input.GetKeyDown (KeyCode.X)) {
			Use ();
		}
	}

	public void Use ()
	{
		if (cooldown <= 0) {
			GetComponent<AudioSource> ().Play ();
			StartCoroutine ("UseCo");
		}
	}

	public IEnumerator UseCo ()
	{

		Debug.Log ("Bullet Fired");
		Instantiate (bulletPrefab, firePoint.position, firePoint.rotation);
		cooldown = cooldownTime;
		playerAnim.SetBool ("isShooting", true);
		ongoingShots++;
		yield return new WaitForSeconds (.4f);
		ongoingShots--;
		if (ongoingShots <= 0)
			playerAnim.SetBool ("isShooting", false);

	}
}