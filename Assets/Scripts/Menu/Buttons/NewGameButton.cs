using UnityEngine;
using System.Collections;

public class NewGameButton : MenuButton
{


	// Use this for initialization
	void Start ()
	{

		IsSelected = true;
		IsUnlocked = true;
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public override void OnSelectButton ()
	{
		Debug.Log ("New Game Button Selected");
		StartCoroutine ("OnSelectButtonContinue");

	} 

	IEnumerator OnSelectButtonContinue ()
	{
		Debug.Log ("Fading to Black");
		ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
		yield return StartCoroutine (sf.FadeToBlack ());
		Application.LoadLevel (1);

	}
}
