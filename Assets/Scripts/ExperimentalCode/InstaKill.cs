using UnityEngine;

/// <summary>
/// If this entity touches the player, the player will die.
/// </summary>
public class InstaKill : MonoBehaviour {
    public void OnTriggerEnter2D(Collider2D other) {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        LevelManager.Instance.KillPlayer();
    }
}