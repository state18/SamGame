using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CoroutineBridge : MonoBehaviour
{

	public static CoroutineBridge instance;

	// Use this for initialization
	void Awake ()
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public  Coroutine myStartCoroutine (IEnumerator routine)
	{
		return StartCoroutine (routine);
	}

	public void myStopCoroutine (Coroutine routine)
	{
        StopCoroutine (routine);
        //StopAllCoroutines();          //working code
	}

    public IEnumerator Wait(float duration) {
        for (float i = 0; i < duration; i += Time.deltaTime) {

            yield return null;
        }
    }
}
