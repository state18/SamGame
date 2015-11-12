using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlinkSingle : MonoBehaviour           //this controls a blinking platform system that toggles on and off (does not linearly travel)
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
		foreach (BlinkPlatform m in blinkPlatforms) {
			m.ToggleState ();
			m.gameObject.SetActive (!m.gameObject.activeInHierarchy);
		}
		
		bool playerInRange = Physics2D.OverlapCircle (soundPosition, soundRadius, isPlayer);
		if (playerInRange)
			blinkSound.Play ();
		//Debug.Log ("finished cycle");
	}
	// Update is called once per frame
	void Update ()
	{
		
	}
}
