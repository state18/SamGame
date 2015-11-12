using UnityEngine;
using System.Collections;

public abstract class  MenuButton : MonoBehaviour
{
	public bool IsSelected{ get; set; }
	public bool IsUnlocked{ get; set; }

	public abstract void OnSelectButton ();
	
}