using UnityEngine;
using System.Collections;

[System.Obsolete("Use GiveDamageToPlayer", true)]
public class HurtPlayerOnContact : MonoBehaviour   //any entity with this will hurt the player on contact
{
    public int damageToGive;                        //how much damage to deal?
    private Player player;
	public bool knockback;
    public bool instaKill;

	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<Player> ();
	}

    void OnTriggerEnter2D (Collider2D other)				//on contact with the player, they are damaged
	{

        if (other.tag == "Player") {
            if (instaKill)
                LevelManager.Instance.KillPlayer();
            else
                player.TakeDamage(damageToGive, gameObject);
        }
		
	}
}
