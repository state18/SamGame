using UnityEngine;
using System.Collections;

/// <summary>
/// Upon touching the player, they will receive an item whose type is specified by an integer.
/// </summary>
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
