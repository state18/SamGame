using UnityEngine;
using System.Collections;

public class MovingMenu : MonoBehaviour
{
	public float scrollRate;
	Transform background;
	public float minX;
	public Vector3 backgroundResetPosition;
	public Vector3 backgroundEndPosition;

	// Use this for initialization
	void Start ()
	{

		background = GetComponent<Transform> ();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (background.position.x < minX) {
			background.position += backgroundResetPosition - backgroundEndPosition;

		}
		background.Translate (-scrollRate * Time.deltaTime, 0f, 0f);


	}
}
