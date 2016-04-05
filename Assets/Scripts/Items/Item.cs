using UnityEngine;

/// <summary>
/// All usable items will inherit from this class.
/// </summary>
public abstract class Item : MonoBehaviour
{

    public bool IsObtained;
    public bool InHand { get; set; }

    /// <summary>
    /// Called when the player presses the Use button.
    /// </summary>
    public abstract void Use();

}
