using UnityEngine;
using System.Collections;

public class LevelTransport : MonoBehaviour
{
	public int levelIndex;

	IEnumerator OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
			yield return StartCoroutine (sf.FadeToBlack ());
			Application.LoadLevel (levelIndex);
		}
	}
}