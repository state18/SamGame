using UnityEngine;

public class GiveDamageToPlayer : MonoBehaviour {
    public int DamageToGive = 10;
    public bool knockback;
    public bool instaKill;
    private Vector2
        _lastPosition,
        _velocity;

    public void LateUpdate() {
        // Performed after all translations in Update method
        _velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
        _lastPosition = transform.position;
    }

    public void OnTriggerStay2D(Collider2D other) {
        var player = other.GetComponent<Player>();
        if (player == null || player.IsDead)
            return;

        if (instaKill) {
            LevelManager.Instance.KillPlayer();
            return;
        }
        var controller = player.GetComponent<CharacterController2D>();
        var totalVelocity = controller.Velocity + _velocity;

        // Handles knockback. The direction and magnitude of the knockback is determined by the velocity of the damage giver,
        // the velocity of the player, and is then multiplied by a scale to determine magnitude.
        // TODO maybe not even use this and stick with my current setup for knockbacks!
        if (knockback && !player.IsInvulnerable) {
            controller.SetForce(new Vector2(
                -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 5, 8, 20),
                -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2, 0, 8)));
        }
       
        player.TakeDamage(DamageToGive, gameObject);
    }
}