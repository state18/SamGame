using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	private Animator anim;
	private bool isActivated = false;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.GetComponent<Player>() != null && !isActivated) {
            LevelManagerProto.Instance.currentCheckPoint = this;
			isActivated = true;
			anim.SetBool ("isActivated", true);
			GetComponent<AudioSource> ().Play ();

		}
	}

    public void SpawnPlayer(Player player) {
        player.RespawnAt(transform);
    }
}
