using UnityEngine;
using System.Collections;

/// <summary>
/// Upon touching this GameObject, the player will collect a coin of a specified type.
/// </summary>
public class CoinPickup : MonoBehaviour
{
	public int coinType;
	CoinManager cm;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			cm = FindObjectOfType <CoinManager> ();
			cm.CollectCoin (coinType);
			AudioSource.PlayClipAtPoint (GetComponent<AudioSource> ().clip, transform.position);
			Destroy (gameObject);
		}
	}
}
