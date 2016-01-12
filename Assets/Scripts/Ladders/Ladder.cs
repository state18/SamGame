using UnityEngine;
using System.Collections;

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

            if (Input.GetAxisRaw("Vertical") != 0)
                cachedPlayer.IsClimbing = true;
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
