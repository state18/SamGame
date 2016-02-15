using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelTransport : MonoBehaviour
{
	public int levelIndex;

	IEnumerator OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
			yield return StartCoroutine (sf.FadeToBlack ());
			SceneManager.LoadScene(levelIndex);
		}
	}
}