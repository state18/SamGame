using UnityEngine;
using System.Collections;

/// <summary>
/// A Game Object with this component will scroll in a specified direction and stay within specified bounds relative to a Transform
/// that will often be referenced in the CameraController component to follow the camera.
/// </summary>
public class MovingMenu : MonoBehaviour {
    public float scrollRate;
    Transform background;
    public Vector3 rightBounds;
    public Vector3 leftBounds;
    private enum Directions { Left, Right, Up, Down };
    [SerializeField]
    private Directions scrollDirection;
    private Vector3 distanceBetweenBounds;
    private CameraController camController;

    // Use this for initialization
    void Start() {

        // keep a reference to the transform to avoid GetComponent every frame.
        background = GetComponent<Transform>();
        camController = Camera.main.GetComponent<CameraController>();
        distanceBetweenBounds = rightBounds - leftBounds;

    }

    // Update is called once per frame
    void Update() {

        // If this Game Object's relative position to its parent is less than the left bounds, move it over to the right bounds.
        // Else if this Game Object's relative position to its parent is greater than the right bounds, move it over to the left bounds.
        if (background.localPosition.x < leftBounds.x) 
            background.localPosition += distanceBetweenBounds;
        else if (background.localPosition.x > rightBounds.x) 
            //Debug.Log("too far to the right");
            background.localPosition -= distanceBetweenBounds;

        // If this object is scrolling to the left, move it over to the left and then account for the camera movement to create perspective.
        if (scrollDirection == Directions.Left)
            background.Translate(-scrollRate * Time.deltaTime - camController.DeltaMovement.x, 0f, 0f);


    }
}
