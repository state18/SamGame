﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Used for backgrounds intended to stay with the camera.
/// Note: Scrolling objects meant to follow the camera need to have this component on their parent GameObject.
/// </summary>
public class FollowCamera : MonoBehaviour {

    [SerializeField]
    private bool followCameraX;
    [SerializeField]
    private bool followCameraY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        var x = followCameraX ? Camera.main.transform.position.x : transform.position.x;
        var y = followCameraY ? Camera.main.transform.position.y : transform.position.y;

        transform.position = new Vector3(x, y, transform.position.z);
	}
}
