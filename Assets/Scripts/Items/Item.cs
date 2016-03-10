using UnityEngine;

public abstract class Item : MonoBehaviour
{

    public bool IsObtained;
    public bool InHand { get; set; }

    public abstract void Use();

}
