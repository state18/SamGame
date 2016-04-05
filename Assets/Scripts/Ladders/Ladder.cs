using UnityEngine;
using System.Collections;

/// <summary>
/// Allows the player to switch to the climbing state if inside. (Needs work)
/// </summary>
public class Ladder : MonoBehaviour {
    bool inside;

    Player cachedPlayer;

    // Use this for initialization
    void Start() {
        cachedPlayer = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (inside) {

            if (Input.GetAxisRaw("Vertical") == 1)
                cachedPlayer.IsClimbing = true;
        }
    }

    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            player.ExitAllTriggers += OnTriggerExit2D;
            inside = true;
            player.LadderColliderCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            player.ExitAllTriggers -= OnTriggerExit2D;
            inside = false;
            player.LadderColliderCount--;
            if (player.LadderColliderCount == 0)
                player.IsClimbing = false;
        }
    }
}
