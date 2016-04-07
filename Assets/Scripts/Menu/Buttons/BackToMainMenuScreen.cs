using UnityEngine;
using System.Collections;

/// <summary>
/// TODO: Rework menu system to a more object oriented approach
/// </summary>
public class BackToMainMenuScreen : MenuButton
{
	MainMenuScreen menuManager;
	ChooseLevel chooseLevel;
	
	// Use this for initialization
	void Start ()
	{
		
		IsSelected = false;
		IsUnlocked = true;
		menuManager = FindObjectOfType<MainMenuScreen> ();
		chooseLevel = FindObjectOfType<ChooseLevel> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public override void OnSelectButton ()
	{
        StartCoroutine(MenuManager.Instance.ChangeScreen("MainMenuScreen"));
		
	}
}
