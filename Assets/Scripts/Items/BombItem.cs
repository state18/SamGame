using UnityEngine;
using System.Collections;

/// <summary>
/// Using this item creates a GameObject with component ActiveBomb.
/// </summary>
public class BombItem : Item
{
	public Transform bombSpawn;
	public GameObject bombPrefab;
	//private float cooldown = 0f;
	public float timeUntilDetonation;
	private int ongoingBombs = 0;         //no more than 2 at a time

    private void Start() {
        IsUtility = true;
    }

    /// <summary>
    /// If there are less than 2 active bombs, spawn one.
    /// </summary>
	public override void Use ()
	{
		if (ongoingBombs < 2) 
			StartCoroutine ("UseCo");
	}

	public IEnumerator UseCo ()
	{
		Debug.Log ("Bomb Dropped");
		Instantiate (bombPrefab, bombSpawn.position, bombSpawn.rotation);
		//cooldown = cooldownTime;
		ongoingBombs++;
		yield return new WaitForSeconds (timeUntilDetonation);
		ongoingBombs--;
		//if (ongoingBombs <= 0)

	}
}
