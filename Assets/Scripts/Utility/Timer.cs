using UnityEngine;
using System.Collections;

public  class Timer : MonoBehaviour
{



	public void StartTimer (float time)
	{

		StartCoroutine ("StartTimeCo", time);

	}

	public IEnumerator StartTimeCo (float time)
	{

		yield return new WaitForSeconds (time);
	}
}
