using UnityEngine;
using System.Collections;

public class ConveyorPlatform : MonoBehaviour
{

	public float speed;

	public void ControllerStay2D(CharacterController2D controller) {

        controller.AddForce(new Vector2(speed, 0));
    }
}
