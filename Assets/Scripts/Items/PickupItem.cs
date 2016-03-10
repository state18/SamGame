using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{

	public int itemID;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.GetComponent<Player>()) {
			ItemManager.Instance.EnableItem (itemID);
			Destroy (gameObject);
		}
	}
}
