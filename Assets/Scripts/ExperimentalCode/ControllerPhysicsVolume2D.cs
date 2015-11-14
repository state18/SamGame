using UnityEngine;
using System.Collections;
/// <summary>
/// Use this class along with a collider with isTrigger enabled to change the way entities behave while inside.
/// Note: Very specific places such as ladders will not make use of this class, but a similar one specifically built for handling that task.
/// This class will handle general parameters such as jumping, velocity, and gravity.
/// </summary>
public class ControllerPhysicsVolume2D : MonoBehaviour {
    public ControllerParameters2D Parameters;
}