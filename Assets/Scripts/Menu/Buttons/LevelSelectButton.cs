using UnityEngine;
using System.Collections;

public class LevelSelectButton : MenuButton
{
	MenuManager menuManager;
	ChooseLevel chooseLevel;
	// Use this for initialization
	void Start ()
	{
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
		yield return StartCoroutine (CameraMoveToLocation.Instance.AnimateMovement("DefaultToLevelSelect"));
		Debug.Log ("moving to level select screen");
		chooseLevel.enabled = true;
		ChooseLevel.EnteringScreen ();
		//ChooseLevel.movementLocked = false;

	}
}
