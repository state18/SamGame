using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the case of the player wanting to climb down a ladder or reach the top
/// </summary>
public class LadderTop : MonoBehaviour {
    // TODO Make smooth transitions when exiting through the tops of ladders. 
    // Maybe have a very brief window of time where an animation plays and prevents the player from moving.
    Player player;
    Vector3 playerScale;
    bool playerInside;
    //Animator anim;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
        //anim = player.GetComponent<Animator>();
        playerScale = player.transform.localScale;
    }

    // IMPORTANT: Change code to not rely on the player's transform being at a specific spot. Use the bounds of the player's box collider and adjust accordingly. (DONE but review in the morning)
    void Update() {
        if (playerInside) {
            //Debug.Log("player inside.");
            if (!player.IsClimbing && Input.GetAxisRaw("Vertical") == -1) {
                var playerColliderBounds = player.GetComponent<BoxCollider2D>().bounds;
                var playerTransformOffsetY = player.transform.position.y - playerColliderBounds.min.y;
                var playerTransformOffsetX = player.transform.position.x - playerColliderBounds.center.x;

                Vector3 desiredDestination = new Vector3(transform.position.x - playerTransformOffsetX, transform.position.y - playerTransformOffsetY + .2f, transform.position.z);

                player.transform.position = desiredDestination;
                player.IsClimbing = true;

            } else if (player.IsClimbing && Input.GetAxisRaw("Vertical") == 1) {

                // anim.SetTrigger("LadderExit");

                var playerColliderBounds = player.GetComponent<BoxCollider2D>().bounds;

                var playerTransformOffsetY = player.transform.position.y - playerColliderBounds.min.y;
                var playerTransformOffsetX = player.transform.position.x - playerColliderBounds.center.x;

                // The amount we translate should be the difference between the destination point and the current center of the player's collider.
                Vector3 desiredDestination = new Vector3(transform.position.x + playerTransformOffsetX, transform.position.y + playerTransformOffsetY, transform.position.z);

                player.transform.position = desiredDestination;
                player.IsClimbing = false;


            }
        }

    }
    void OnTriggerEnter2D(Collider2D other) {

        if (other.GetComponent<Player>()) {
            playerInside = true;
            player.ExitAllTriggers += OnTriggerExit2D;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<Player>()) {
            playerInside = false;
            player.ExitAllTriggers -= OnTriggerExit2D;
        }
    }
}
