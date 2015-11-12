using UnityEngine;
using System.Collections;

public class PistolItem : Item
{
	public Transform firePoint;
	public GameObject bulletPrefab;	
	private Animator playerAnim;
	private PlayerController pc;

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
		playerAnim = firePoint.GetComponentInParent<Animator> ();
		pc = firePoint.GetComponentInParent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;

		if (InHand && Input.GetKeyDown (KeyCode.X) && !pc.isClimbing) {
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