using UnityEngine;
using System.Collections;

// Currently just a test class to automate the background wrapping.
public class WrapWithCamera : MonoBehaviour {

    [SerializeField]
    private bool wrapX;
    [SerializeField]
    private bool wrapY;

    [SerializeField]
    private float horizontalDistanceUntilWrap;
    [SerializeField]
    private float verticalDistanceUntilWrap;

    private float leftBound;
    private float rightBound;
    private float upperBound;
    private float lowerBound;

    private Vector2 totalCameraProgression;

	// Use this for initialization
	void Start () {
        totalCameraProgression = Vector2.zero;
        leftBound = transform.localPosition.x - horizontalDistanceUntilWrap;
        rightBound = transform.localPosition.x + horizontalDistanceUntilWrap;
        lowerBound = transform.localPosition.y - verticalDistanceUntilWrap;
        upperBound = transform.localPosition.y + verticalDistanceUntilWrap;

	}
	
	// Update is called once per frame
	void Update () {

        totalCameraProgression += Camera.main.GetComponent<CameraController>().DeltaMovement;
        if (wrapX) {
            if(totalCameraProgression.x < leftBound) {
                transform.localPosition -= new Vector3(horizontalDistanceUntilWrap, 0f);
                rightBound = leftBound;
                leftBound -= horizontalDistanceUntilWrap;
            } else if (totalCameraProgression.x > rightBound){
                transform.localPosition += new Vector3(horizontalDistanceUntilWrap, 0f);
                leftBound = rightBound;
                rightBound += horizontalDistanceUntilWrap;
            }
        }

        if (wrapY) {
            if(totalCameraProgression.y < lowerBound) {
                Debug.Log("below lower bound");
                transform.localPosition -= new Vector3(0f, verticalDistanceUntilWrap);
                upperBound = lowerBound;
                lowerBound -= verticalDistanceUntilWrap;
            } else if (totalCameraProgression.y > upperBound) {
                Debug.Log("above upper bound");
                transform.localPosition += new Vector3(0f, verticalDistanceUntilWrap);
                lowerBound = upperBound;
                upperBound += verticalDistanceUntilWrap;
            }
        }

        
    }
}
