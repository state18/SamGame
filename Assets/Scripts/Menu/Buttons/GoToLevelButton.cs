using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToLevelButton : MenuButton
{

	public int levelToLoad;

	// Use this for initialization
	void Start ()
	{
		
		IsSelected = false;
		IsUnlocked = true;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public override void OnSelectButton ()
	{
		Debug.Log ("level Button Selected" + levelToLoad);
		StartCoroutine ("OnSelectButtonContinue");
		
	} 
	
	IEnumerator OnSelectButtonContinue ()
	{
		Debug.Log ("Fading to Black");
		ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
		yield return StartCoroutine (sf.FadeToBlack ());
		SceneManager.LoadScene(levelToLoad);
		
	}
}
