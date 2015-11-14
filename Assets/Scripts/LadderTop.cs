using UnityEngine;
using System.Collections;

public class LadderTop : MonoBehaviour {
    // TODO As of 11/13/2015, this object's collision is on Ground layer, adjust accordingly and tweak the transitions.
    Player player;

    Animator anim;

    public float yOffset;

    // Use this for initialization
    void Start() {
        player = FindObjectOfType<Player>();
        anim = player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Player") {


            if (!player.IsClimbing && Input.GetAxisRaw("Vertical") == -1) {

                other.transform.position = new Vector3(transform.position.x, other.transform.position.y, other.transform.position.z);
                other.transform.Translate(0f, yOffset, 0f);

                player.CanClimb = true;
                player.IsClimbing = true;

            } else if (player.IsClimbing && Input.GetAxisRaw("Vertical") == 1) {

                anim.SetTrigger("LadderExit");
                other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
                other.transform.Translate(0f, -yOffset, 0f);


                player.CanClimb = false;
                player.IsClimbing = false;


            }
        }

    }
}
