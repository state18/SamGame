using UnityEngine;
using System.Collections;

public class DestroyObjectOverTime : MonoBehaviour
{

	public float lifetime;

	// Use this for initialization
	void Start ()
	{						//this Script is for destroying projectiles after some time
	
	}
	
	// Update is called once per frame
	void Update ()
	{

		lifetime -= Time.deltaTime;

		if (lifetime < 0) {
			Destroy (gameObject);
		}
	}
}
