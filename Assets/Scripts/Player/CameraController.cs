using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour  //This script controls the camera movement and will not allow it to go out of bounds. It will follow the player most of the time.
{
    public Transform Player;                    //reference to the player

    public Transform backgroundLayerNotP;       //background to stay with camera

    public Vector2                              //margin for player to walk in without camera movement
        Margin,
        Smoothing;                              //how fast will the camera move to catch up?

    public BoxCollider2D Bounds;                //marks the bounds of the level

    private Vector3                             //holds values for the bounds above
        _min,
        _max;

    public bool IsFollowing { get; set; }                   //is the camera following the player?

    public void Start()                     //bound values initialized
    {
        Player = FindObjectOfType<Player>().transform;
        _min = Bounds.bounds.min;
        _max = Bounds.bounds.max;
        IsFollowing = true;
    }

    public void Awake() {
        //Camera.main.orthographicSize = Screen.height / (2f * 16);
    }
    public void FixedUpdate() {

    }
    public void LateUpdate() {
        var x = transform.position.x;               //controls the position of the camera (x and y)
        var y = transform.position.y;

        if (IsFollowing) {

            if (Mathf.Abs(x - Player.position.x) > Margin.x)                                //if the player strays too far away from margin, camera moves to the player
                x = Mathf.Lerp(x, Player.position.x, Smoothing.x * Time.deltaTime);

            if (Mathf.Abs(y - Player.position.y) > Margin.y)
                y = Mathf.Lerp(y, Player.position.y, Smoothing.y * Time.deltaTime);

        }

        var cameraHalfWidth = Camera.main.orthographicSize * ((float)Screen.width / Screen.height);     //half of the camera size used for calculations

        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);                     //clamps the camera to the bounds

        y = Mathf.Clamp(y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

        transform.position = new Vector3(x, y, transform.position.z);

        if (backgroundLayerNotP != null) {
            backgroundLayerNotP.position = new Vector3(transform.position.x, transform.position.y, backgroundLayerNotP.position.z);
        }
        //keeps camera from leaving bounds by adding above values
    }
}
