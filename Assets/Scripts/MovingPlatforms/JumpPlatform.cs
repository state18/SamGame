using UnityEngine;
using System.Collections;
/// <summary>
/// If something with a CharacterController2D lands on this platform, they will automatically jump.
/// </summary>
public class JumpPlatform : MonoBehaviour {
    public float JumpMagnitude = 20;

    /// <summary>
    /// Upon entering the platform, make the entity jump.
    /// </summary>
    /// <param name="controller"> the controller of the entity on the platform</param>
    public void ControllerEnter2D(CharacterController2D controller) {
        controller.SetVerticalForce(JumpMagnitude);
    }
}