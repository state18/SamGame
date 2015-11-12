using UnityEngine;
using System.Collections;

public class CameraMoveToLocation : MonoBehaviour
{

	Animator anim;
	bool isMoving = false;
	
	// Use this for initialization
	void Start ()
	{
		
		anim = GetComponent<Animator> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	public IEnumerator DefaultToLevelSelect ()
	{
		isMoving = true;
		Debug.Log ("before triggering");
		anim.SetTrigger ("DefaultToLevelSelect");
		Debug.Log ("animation running");
		while (isMoving)
			yield return null;
	}
	
	public IEnumerator LevelSelectToDefault ()
	{
		isMoving = true;
		anim.SetTrigger ("LevelSelectToDefault");
		
		while (isMoving)
			yield return null;
	}
	void AnimationComplete ()
	{
		isMoving = false;
	}
}
