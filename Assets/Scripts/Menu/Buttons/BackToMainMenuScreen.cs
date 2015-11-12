using UnityEngine;
using System.Collections;

public class BackToMainMenuScreen : MenuButton
{
	CameraMoveToLocation camMover;
	MenuManager menuManager;
	ChooseLevel chooseLevel;
	
	// Use this for initialization
	void Start ()
	{
		
		IsSelected = false;
		IsUnlocked = true;
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
		ChooseLevel.LeavingScreen ();
		chooseLevel.enabled = false;
		StartCoroutine ("OnSelectButtonContinue");
		
	} 
	
	IEnumerator OnSelectButtonContinue ()
	{

		Debug.Log ("before yielding");
		yield return StartCoroutine (camMover.LevelSelectToDefault ());
		menuManager.enabled = true;
		MenuManager.EnteringScreen ();
	}
}
