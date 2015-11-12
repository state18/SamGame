using UnityEngine;
using System.Collections;

public class CoinPickup : MonoBehaviour
{
	public int coinType;
	CoinManager cm;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

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
