using UnityEngine;

/// <summary>
/// All usable items will inherit from this class.
/// </summary>
public abstract class Item : MonoBehaviour
{
    public int index;
    public bool IsObtained;
    public bool InHand { get; set; }
    public bool IsUtility;

    /// <summary>
    /// Called when the player presses the Use button.
    /// </summary>
    public abstract void Use();

    public bool Equals(Object o) {
        var otherItem = o as Item;
        return otherItem != null && otherItem.index == index;
    }
}
