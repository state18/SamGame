using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlinkPlatformSystem : MonoBehaviour
{
	public float timeBetween;
	public float soundRadius;

	Vector3 soundPosition;
	AudioSource blinkSound;

	public int[] activePlatforms;
	public BlinkPlatform[] blinkPlatforms;
	public LayerMask isPlayer;

	// Use this for initialization
	void Start ()
	{
		if (blinkPlatforms != null && activePlatforms != null && activePlatforms.Length <= blinkPlatforms.Length) {

			foreach (BlinkPlatform m in blinkPlatforms) {
				m.ToggleState ();
				m.gameObject.SetActive (false);
				//Debug.Log ("disabled");
			}
			foreach (int n in activePlatforms) {
				blinkPlatforms [n].gameObject.SetActive (true);
				blinkPlatforms [n].ToggleState ();
				//Debug.Log ("enabled" + n);
			}
		}
		blinkSound = GetComponentInChildren<AudioSource> ();
		soundPosition = blinkSound.transform.position;
		InvokeRepeating ("CyclePlatforms", 1f, timeBetween);
	}

	public void CyclePlatforms ()
	{
		for (int i = activePlatforms.Length - 1; i >= 0; i--) {
			//Debug.Log ("started" + i);
			blinkPlatforms [activePlatforms [i]].ToggleState ();
			blinkPlatforms [activePlatforms [i]].gameObject.SetActive (false);
			activePlatforms [i]++;
			if (activePlatforms [i] >= blinkPlatforms.Length) {
				activePlatforms [i] = 0;
			} else if (activePlatforms [i] < 0) {
				activePlatforms [i] = blinkPlatforms.Length - 1;
			}
			blinkPlatforms [activePlatforms [i]].gameObject.SetActive (true);
			blinkPlatforms [activePlatforms [i]].ToggleState ();
		}

		bool playerInRange = Physics2D.OverlapCircle (soundPosition, soundRadius, isPlayer);
        if (playerInRange) {
            Debug.Log("Playing blink sound.");
            blinkSound.Play();
        }
		//Debug.Log ("finished cycle");
	}

}
