using UnityEngine;
using System.Collections;

public class LevelSelectButton : MenuButton
{
	CameraMoveToLocation camMover;
	MenuManager menuManager;
	ChooseLevel chooseLevel;
	// Use this for initialization
	void Start ()
	{
		camMover = Camera.main.GetComponent<CameraMoveToLocation> ();
		menuManager = FindObjectOfType<MenuManager> ();
		chooseLevel = FindObjectOfType<ChooseLevel> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public override void OnSelectButton ()
	{
		MenuManager.LeavingScreen ();
		menuManager.enabled = false;
		StartCoroutine (ButtonFunction ());
	} 

	IEnumerator ButtonFunction ()
	{
		Debug.Log ("before yielding");
		yield return StartCoroutine (camMover.DefaultToLevelSelect ());
		Debug.Log ("moving to level select screen");
		chooseLevel.enabled = true;
		ChooseLevel.EnteringScreen ();
		//ChooseLevel.movementLocked = false;

	}
}
