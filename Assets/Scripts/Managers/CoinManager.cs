using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Tracks the number of yellow coins collected, and if the blue coin has been collected
/// </summary>
public class CoinManager : MonoBehaviour {

    public static int coinTotal;
    public Text coinText;
    public Toggle blueCoin;

    /// <summary>
    /// Adds the appropriate coin to the obtained coins
    /// </summary>
    /// <param name="coinType">type of coin obtained</param>
    public void CollectCoin(int coinType) {
        if (coinType == 0) {
            coinTotal++;
            coinText.text = "0" + coinTotal;

        } else if (coinType == 1) {
            blueCoin.isOn = true;
        }
    }
}
