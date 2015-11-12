using UnityEngine;
using System.Collections;

public class WaitCo : MonoBehaviour
{

	public static IEnumerator Wait (float duration)
	{
		for (float i = 0; i < duration; i += Time.deltaTime) {

			yield return null;
		}
	} 
}
