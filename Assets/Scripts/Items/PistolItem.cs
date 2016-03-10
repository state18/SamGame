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
    // private CharacterController2D playerController;
    public Projectile projectile;

	private int ongoingShots = 0;
	private float cooldown = 0f;
	public float cooldownTime;

	// Use this for initialization
	void Start ()
	{
        player = FindObjectOfType<Player>();
        // playerController = player.GetComponent<CharacterController2D>();
        playerAnim = player.GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
	}

	public override void Use ()
	{
		if (cooldown <= 0 && !player.IsClimbing) {
			GetComponent<AudioSource> ().Play ();
			StartCoroutine ("UseCo");
		}
	}

	public IEnumerator UseCo ()
	{

		Debug.Log ("Bullet Fired");
		var newBullet = (SimpleProjectile)Instantiate (projectile, firePoint.position, firePoint.rotation);
        var direction = player.IsFacingRight ? Vector2.right : -Vector2.right;
        //var initialVelocity = playerController.Velocity;
        newBullet.Initialize(player.gameObject, direction);
		cooldown = cooldownTime;
		playerAnim.SetBool ("isShooting", true);
		ongoingShots++;
		yield return new WaitForSeconds (.4f);
		ongoingShots--;
		if (ongoingShots <= 0)
			playerAnim.SetBool ("isShooting", false);

	}
}