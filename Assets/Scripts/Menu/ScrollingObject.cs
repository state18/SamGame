using UnityEngine;
using System.Collections;

/// <summary>
/// A Game Object with this component will scroll in a specified direction and stay within specified bounds.
/// </summary>
public class ScrollingObject : MonoBehaviour {
    public float horizontalScrollRate;
    public float verticalScrollRate;
    // The higher the smoothing distance, the less the object will move based on the camera.
    public float smoothingDistance;
    Transform background;
    public float rightBounds;
    public float leftBounds;
    public float upBounds;
    public float downBounds;
    private enum HorizontalDirections { None, Left, Right };
    private enum VerticalDirections { None, Down, Up };
    [SerializeField]
    private HorizontalDirections horizontalScrollDirection;
    [SerializeField]
    private VerticalDirections verticalScrollDirection;
    private float horizontalDistanceBetweenBounds;
    private float verticalDistanceBetweenBounds;
    private CameraController camController;

    // Use this for initialization
    void Start() {

        // keep a reference to the transform to avoid GetComponent every frame.
        background = GetComponent<Transform>();
        camController = Camera.main.GetComponent<CameraController>();

        horizontalDistanceBetweenBounds = rightBounds - leftBounds;
        verticalDistanceBetweenBounds = upBounds - downBounds;
    }

    // Update is called once per frame
    void Update() {

        // If this Game Object's relative position to its parent is less than the left bounds, move it over to the right bounds.
        // Else if this Game Object's relative position to its parent is greater than the right bounds, move it over to the left bounds.


        if (background.localPosition.x < leftBounds)
            background.localPosition += new Vector3(horizontalDistanceBetweenBounds, 0f, 0f);
        else if (background.localPosition.x > rightBounds)
            //Debug.Log("too far to the right");
            background.localPosition -= new Vector3(horizontalDistanceBetweenBounds, 0f, 0f);


        if (verticalScrollDirection != VerticalDirections.None) {
            if (background.localPosition.y < downBounds)
                background.localPosition += new Vector3(0f, verticalDistanceBetweenBounds, 0f);
            else if (background.localPosition.y > upBounds)
                background.localPosition -= new Vector3(0f, verticalDistanceBetweenBounds, 0f);
        }
        var camMovement = camController != null ? camController.DeltaMovement : Vector2.zero;

        // Execute the scrolling
        // If this object is scrolling to the left, move it over to the left and then account for the camera movement to create perspective.
        if (horizontalScrollDirection == HorizontalDirections.Left) {
            // Accounting for camera movement and then multiplying it by the smoothing factor gives the illusion of parallax.
            background.Translate(-horizontalScrollRate * Time.deltaTime - camMovement.x * smoothingDistance, 0f, 0f);
        } else if (horizontalScrollDirection == HorizontalDirections.Right)
            background.Translate(horizontalScrollRate * Time.deltaTime + camMovement.x * smoothingDistance, 0f, 0f);

        if (verticalScrollDirection == VerticalDirections.Up)
            background.Translate(0f, verticalScrollRate * Time.deltaTime + camMovement.y * smoothingDistance, 0f);
        else if (verticalScrollDirection == VerticalDirections.Down)
            background.Translate(0f, -verticalScrollRate * Time.deltaTime - camMovement.y * smoothingDistance, 0f);


    }
}
