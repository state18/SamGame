using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour
{

	public bool IsObtained{ get; set; }
	public bool InHand{ get; set; }


	//This is where the actual function of the item in the game
	//will be activated.

	//public abstract void Use ();

	/*/////////////////////////////
	//Setters

	public void setIsObtained (bool b)
	{
		isObtained = b;
	}

	public void setInHand (bool b)
	{
		inHand = b;
	}

	//Getters

	public bool getInHand ()
	{

		return inHand;
	}

	public bool getIsObtained ()
	{
		
		return isObtained;
	}

	*//////////////////////

}
