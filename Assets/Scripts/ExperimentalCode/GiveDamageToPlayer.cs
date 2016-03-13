using UnityEngine;

public class GiveDamageToPlayer : MonoBehaviour {
    public int DamageToGive = 10;
    public bool knockback;
    public bool instaKill;

    private bool playerInside;

    void Update () {
        if (playerInside) {
            if (instaKill) {
                LevelManager.Instance.KillPlayer();
                return;
            }

            // Handles knockback. The direction and magnitude of the knockback is determined by the velocity of the damage giver,
            // the velocity of the player, and is then multiplied by a scale to determine magnitude.
            // TODO Possibly make this less player-centric and just look for a IKnockbackable interface.
            if (knockback && !Player.Instance.IsInvulnerable) {
                //Debug.Log("knocking back player");
                Player.Instance.Knockback(transform.position);
            }

            Player.Instance.TakeDamage(DamageToGive, gameObject);
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.GetComponent<Player>() != null) {
            Player.Instance.ExitAllTriggers += OnTriggerExit2D;
            playerInside = true;
        }
    }

    void OnTriggerExit2D (Collider2D other) {
        Player.Instance.ExitAllTriggers -= OnTriggerExit2D;
        playerInside = false;
    }
}