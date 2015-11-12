using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{

	public static int coinTotal;
	public Text coinText;
	public Toggle blueCoin;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void CollectCoin (int coinType)
	{
		if (coinType == 0) {
			coinTotal++;
			coinText.text = "0" + coinTotal;

		} else if (coinType == 1) {
			blueCoin.isOn = true;
		}
	}
}
