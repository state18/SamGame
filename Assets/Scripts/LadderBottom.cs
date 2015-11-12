using UnityEngine;
using System.Collections;

public class LadderBottom : MonoBehaviour {
    bool inside;

    Player player;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update() {
        if (inside) {

            if (Input.GetAxisRaw("Vertical") == -1) {

                //player.isClimbing = false;

            } else if (Input.GetAxisRaw("Vertical") == 1) {

                //player.isClimbing = true;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "PlayerTrigger")
            inside = true;
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "PlayerTrigger")
            inside = false;
    }
}
