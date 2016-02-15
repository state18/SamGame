﻿using UnityEngine;
using System.Collections;

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

    void Update() {
        if (playerInside) {
            //Debug.Log("player inside.");
            if (!player.IsClimbing && Input.GetAxisRaw("Vertical") == -1) {
                var yOffset = player.GetComponent<BoxCollider2D>().size.y / 2 * playerScale.y;
                // The amount we translate should be the difference between the destination point and the current center of the player's collider.
                Vector3 desiredDestination = new Vector3(transform.position.x, transform.position.y - yOffset + .2f, transform.position.z);

                player.transform.position = desiredDestination;
                player.IsClimbing = true;

            } else if (player.IsClimbing && Input.GetAxisRaw("Vertical") == 1) {

                // anim.SetTrigger("LadderExit");
                var yOffset = player.GetComponent<BoxCollider2D>().size.y / 2 * playerScale.y;
                // The amount we translate should be the difference between the destination point and the current center of the player's collider.
                Vector3 desiredDestination = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

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
