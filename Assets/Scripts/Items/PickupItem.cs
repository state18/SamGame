using UnityEngine;
using System.Collections;

/// <summary>
/// Upon touching the player, they will receive an item whose type is specified by an integer.
/// TODO: make this class hold a reference to an item gameobject and not just an integer.
/// </summary>
public class PickupItem : MonoBehaviour
{

	public Item item;

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.GetComponent<Player>()) {
			ItemManager.Instance.EnableItem (item);
			Destroy (gameObject);
		}
	}
}
