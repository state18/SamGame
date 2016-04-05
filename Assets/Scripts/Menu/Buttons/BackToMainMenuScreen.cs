using UnityEngine;
using System.Collections;

/// <summary>
/// TODO: Rework menu system to a more object oriented approach
/// </summary>
public class BackToMainMenuScreen : MenuButton
{
	MenuManager menuManager;
	ChooseLevel chooseLevel;
	
	// Use this for initialization
	void Start ()
	{
		
		IsSelected = false;
		IsUnlocked = true;
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
		yield return StartCoroutine (CameraMoveToLocation.Instance.AnimateMovement("LevelSelectToDefault"));
		menuManager.enabled = true;
		MenuManager.EnteringScreen ();
	}
}
