using UnityEngine;
using System.Collections;

public class BlinkPlatform : MonoBehaviour
{

	public bool Activated{ get; set; }


	void Awake ()
	{
		Activated = false;
		//audSource = GetComponent<AudioSource> ();
	}
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void ToggleState ()
	{
	

	}
}
