using UnityEngine;
using System.Collections;

// TODO rework the on/ off methods into a single toggle method.
public class LeverBehavior : MonoBehaviour {
    public DoorController door;
    public bool isOn;
    bool playerInside;
    Animator anim;

    // Use this for initialization
    void Start() {
        isOn = false;
        playerInside = false;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update() {

        if (playerInside && Input.GetButtonDown("Up"))
            ToggleDoor();

    }

    public void ToggleDoor() {

        if (door == null) {
            Debug.Log("No door attached!");
            return;
        }

        if (isOn) {
            isOn = false;
            anim.SetBool("On", false);
            door.CloseDoor();
            GetComponent<AudioSource>().Play();
        } else {
            isOn = true;
            anim.SetBool("On", true);
            door.OpenDoor();
            GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            playerInside = true;
        } else if (other.tag == "Projectile") {
            ToggleDoor();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") {
            playerInside = false;
        }
    }
}
