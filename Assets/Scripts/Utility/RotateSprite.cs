using UnityEngine;
using System.Collections;

public class RotateSprite : MonoBehaviour
{

	float rotationsPerMinute = 28;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate (0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
	}
}
