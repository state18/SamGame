﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the movement of the main camera.
/// </summary>
public class CameraController : MonoBehaviour  //This script controls the camera movement and will not allow it to go out of bounds. It will follow the player most of the time.
{
    public Vector2 Smoothing;                              //how fast will the camera move to catch up?
    public BoxCollider2D Bounds;                //marks the bounds of the level
    private Vector3                             //holds values for the bounds above
        _min,
        _max;

    public bool IsFollowing { get; set; }                   //is the camera following the player?
    public Vector2 DeltaMovement { get; private set; } //Stores the movement of the camera from the previous frame to the current one.

    public Action CameraPositionUpdated;

    void Start()                     //bound values initialized
    {
        _min = Bounds.bounds.min;
        _max = Bounds.bounds.max;
        IsFollowing = true;
    }

    void LateUpdate() {

        var lastPosition = transform.position;

        var x = transform.position.x;               //controls the position of the camera (x and y)
        var y = transform.position.y;

        if (IsFollowing) {
            // Move towards the player's position. Note: This used to only happen if the player stepped outside of the margins for leeway. That did not cooperate with very slow
            // movement so it has been removed. 
            x = Mathf.Lerp(x, Player.Instance.transform.position.x, Smoothing.x * Time.deltaTime);

            y = Mathf.Lerp(y, Player.Instance.transform.position.y, Smoothing.y * Time.deltaTime);

            //x = Player.Instance.transform.position.x;
            //y = Player.Instance.transform.position.y;

        }

        var cameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);     //half of the camera size used for calculations

        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);                     //clamps the camera to the bounds

        y = Mathf.Clamp(y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

        transform.position = new Vector3(x, y, transform.position.z);

        DeltaMovement = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);

        if (CameraPositionUpdated != null)
            CameraPositionUpdated();
    }
}
