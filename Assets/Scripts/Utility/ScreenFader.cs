using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
	Animator anim;
	bool isFading = false;

	// Use this for initialization
	void Start ()
	{
        instance = this;
		anim = GetComponent<Animator> ();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	public IEnumerator FadeToClear ()
	{
		isFading = true;
		anim.SetTrigger ("FadeIn");

		while (isFading)
			yield return null;
	}

	public IEnumerator FadeToBlack ()
	{
		isFading = true;
		anim.SetTrigger ("FadeOut");
		
		while (isFading)
			yield return null;
	}
	void AnimationComplete ()
	{
		isFading = false;
	}
}
