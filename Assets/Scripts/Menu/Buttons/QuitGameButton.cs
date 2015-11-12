using UnityEngine;
using System.Collections;

public class QuitGameButton : MenuButton
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public override void OnSelectButton ()
	{
		Application.Quit ();
	} 
}
