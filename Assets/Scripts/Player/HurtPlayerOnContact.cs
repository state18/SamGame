using UnityEngine;
using System.Collections;

[System.Obsolete("Use GiveDamageToPlayer")]
public class HurtPlayerOnContact : MonoBehaviour   //any entity with this will hurt the player on contact
{
    public int damageToGive;                        //how much damage to deal?
    private Player player;
	public bool knockback;

	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<Player> ();
	}

    void OnTriggerEnter2D (Collider2D other)				//on contact with the player, they are damaged
	{

		if (other.tag == "Player") {
            player.TakeDamage(damageToGive, gameObject);
		}
	}
}
