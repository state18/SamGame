using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {
    bool inside;

    Player player;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (inside) {

            if (Input.GetAxisRaw("Vertical") != 0)
                player.IsClimbing = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
            inside = true;
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            inside = false;
            player.IsClimbing = false;
        }
    }
}
