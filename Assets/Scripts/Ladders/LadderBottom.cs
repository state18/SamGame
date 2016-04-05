using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the case of the player wanting to enter/exit a ladder from the bottom
/// </summary>
public class LadderBottom : MonoBehaviour {
    bool inside;

    Player cachedPlayer;

    // Use this for initialization
    void Start() {
        cachedPlayer = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (inside) {

            if (Input.GetAxisRaw("Vertical") == -1 && cachedPlayer.LadderColliderCount == 0) {

                cachedPlayer.IsClimbing = false;

            } else if (Input.GetAxisRaw("Vertical") == 1) {

                cachedPlayer.IsClimbing = true;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            inside = true;
            player.LadderColliderCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Player player = other.GetComponent<Player>();
        if (player != null) {
            inside = false;
            player.LadderColliderCount--;
            if (player.LadderColliderCount == 0)
                player.IsClimbing = false;
        }
    }
}
