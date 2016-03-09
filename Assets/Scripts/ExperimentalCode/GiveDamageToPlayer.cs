using UnityEngine;

public class GiveDamageToPlayer : MonoBehaviour {
    public int DamageToGive = 10;
    public bool knockback;
    public bool instaKill;

    public void OnTriggerStay2D(Collider2D other) {
        var player = other.GetComponent<Player>();

        if (player == null || player.IsDead)
            return;

        if (instaKill) {
            LevelManager.Instance.KillPlayer();
            return;
        }

        // Handles knockback. The direction and magnitude of the knockback is determined by the velocity of the damage giver,
        // the velocity of the player, and is then multiplied by a scale to determine magnitude.
        if (knockback && !player.IsInvulnerable) {
            //Debug.Log("knocking back player");
            player.Knockback(transform.position);
        }
       
        player.TakeDamage(DamageToGive, gameObject);
    }
}