using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{

	public int itemID;

	//private ItemManager itemManager;

	// Use this for initialization
	void Start ()
	{
		//itemManager = FindObjectOfType<ItemManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {

			ItemManager.EnableItem (itemID);
			Destroy (gameObject);
		}
	}
}
