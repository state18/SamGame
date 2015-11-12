using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour
{

	public Transform[] backgroundLayersP;

	public float parallaxScale;
	public float parallaxReductionFactor;
	public float smoothing;

	private Vector3 _lastPosition;

	// Use this for initialization
	void Start ()
	{
		_lastPosition = transform.position;

	}
	
	// Update is called once per frame
	void Update ()
	{

		var parallax = (_lastPosition.x - transform.position.x) * parallaxScale;

		for (var i = 0; i <backgroundLayersP.Length; i++) {


			var backgroundTargetPosition = backgroundLayersP [i].position.x + parallax * (i * parallaxReductionFactor + 1);
			backgroundLayersP [i].position = Vector3.Lerp (
				backgroundLayersP [i].position, new Vector3 (
				backgroundTargetPosition, backgroundLayersP [i].position.y, backgroundLayersP [i].position.z
			), smoothing * Time.deltaTime);

		}

		_lastPosition = transform.position;
	}


}
