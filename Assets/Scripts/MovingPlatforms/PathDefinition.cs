using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathDefinition : MonoBehaviour
{

	public Transform[] pathMarkers;

	public FollowType type;

	public enum FollowType
	{
		Cycle,
		PingPong
	}

	public IEnumerator<Transform> GetPathEnumerator ()
	{
		if (pathMarkers == null || pathMarkers.Length < 1)
			yield break;

		var direction = 1;
		var index = 0;

		if (type == FollowType.PingPong) {
			while (true) {
				yield return pathMarkers [index];
			
				if (pathMarkers.Length == 1)
					continue;
			
				if (index <= 0)
					direction = 1;
				else if (index >= pathMarkers.Length - 1)
					direction = -1;
			
				index += direction;
			}
		} else if (type == FollowType.Cycle) {

			while (true) {
				yield return pathMarkers [index];

				if (pathMarkers.Length == 1)
					continue;

				if (index >= pathMarkers.Length - 1) {
					index = 0;
				} else {
					index += direction;
				}
			}
		}


	}

	public void OnDrawGizmos ()
	{
		
		if (pathMarkers == null || pathMarkers.Length < 2)
			return;
		
		var points = pathMarkers.Where (t => t != null).ToList ();
		if (points.Count < 2)
			return;
		
		for (var i = 1; i < points.Count; i++) {
			Gizmos.DrawLine (points [i - 1].position, points [i].position);
		}
	}
	
}
