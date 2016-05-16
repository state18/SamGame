using UnityEngine;
using System.Collections;
using UnityEngine.UI;
// Active items can be cycled through. 
// Only one passive item may be held at a time.
// 0 = no item in use (hand)
// 1 = pistol
// 2 = bombs

/// <summary>
/// Connects the items HUD with the item logic, and the player with the items
/// </summary>
public class ItemManager : MonoBehaviour {
    public static ItemManager Instance;

    public GameObject pistol;
    public GameObject hand;
    public GameObject bombs;

    public Image handHUD;
    public Image pistolHUD;
    public Image bombHUD;

    private const int offensiveSize = 2;
    private const int utilitySize = 1;

    private GameObject[] offensiveItems;
    private GameObject[] utilityItems;

    private Image[] activeOffensiveItemHUD;
    private Image[] activeUtilityItemHUD;

    private int currentOffensiveIndex;
    private int currentUtilityIndex;

    private Player player;

    void Awake() {
        Instance = this;
    }

    // Use this for initialization
    void Start() {

        player = FindObjectOfType<Player>();

        // Hard code the following as more items are added to the game. This structure will never change throughout the game.
        // TODO Perhaps reading in an XML file that contains level data would be a better of doing this.
        // TODO Instead of tracking currentOffensiveIndex and currentUtilityIndex, perhaps track the active items themselves.

        offensiveItems = new GameObject[offensiveSize];
        activeOffensiveItemHUD = new Image[offensiveSize];
        currentOffensiveIndex = 0;

        utilityItems = new GameObject[utilitySize];
        activeUtilityItemHUD = new Image[utilitySize];
        currentUtilityIndex = 0;

        offensiveItems[0] = hand;
        offensiveItems[1] = pistol;

        utilityItems[0] = bombs;

        // Handle offensive item HUD
        activeOffensiveItemHUD[0] = handHUD;
        activeOffensiveItemHUD[1] = pistolHUD;

        // Handle utility item HUD
        activeUtilityItemHUD[0] = bombHUD;

        // Hand item is initialized. (Player will always have the sword)
        offensiveItems[0].GetComponent<Item>().IsObtained = true;
        offensiveItems[0].GetComponent<Item>().InHand = true;
        activeOffensiveItemHUD[0].GetComponent<Image>().enabled = true;

        // Utility slot is initialized with bomb item if the player already has this item.
        // TODO Later on this should remember the item the player last had equipped for this slot (Read from save file?)
        if (utilityItems[0].GetComponent<Item>().IsObtained) {
            utilityItems[0].GetComponent<Item>().InHand = true;
            activeUtilityItemHUD[0].GetComponent<Image>().enabled = true;
        }

        //items [1].GetComponent<Item> ().IsObtained = true;
        //items [2].GetComponent<Item> ().IsObtained = true;

    }

    // Update is called once per frame
    void Update() {
        // The only time a player can't swap their items is when they're dead (or in the future, when a cutscene is active)
        // TODO Don't use KeyCode, make use of the virtual buttons
        if (!player.IsDead) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                CycleItem(offensiveItems[currentOffensiveIndex].GetComponent<Item>());
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                CycleItem(utilityItems[currentUtilityIndex].GetComponent<Item>());
            }
        }

        // Check to see if offensive item should be used
        if (Input.GetButtonDown("UseOffensiveItem") && player.CanUseItems)
            offensiveItems[currentOffensiveIndex].GetComponent<Item>().Use();

        // Check to see if utility item should be used
        if (Input.GetButtonDown("UseUtilityItem") && utilityItems[currentUtilityIndex].GetComponent<Item>().IsObtained && player.CanUseItems)
            utilityItems[currentUtilityIndex].GetComponent<Item>().Use();

    }
    /// <summary>
    /// Selects the "next" item (in the offensive or utility list)
    /// </summary>
    /// <param name="item">current item to cycle from</param>
    public void CycleItem(Item item) {
        var index = item.index;
        var itemArray = item.IsUtility ? utilityItems : offensiveItems;
        var itemHUD = item.IsUtility ? activeUtilityItemHUD : activeOffensiveItemHUD;

        itemArray[index].GetComponent<Item>().InHand = false;
        itemHUD[index].GetComponent<Image>().enabled = false;

        // For utility items, there could be none obtained, so after all items are checked, the loop will exit.
        var numChecked = 0;
        do {
            // There is no obtained item in this array. (Will happen before player unlocks a utility item.)
            if (numChecked++ > itemArray.Length)
                return;

            index += 1;
            if (index > itemArray.Length - 1) 
                index = 0;

        } while (!itemArray[index].GetComponent<Item>().IsObtained);

        itemArray[index].GetComponent<Item>().InHand = true;
        itemHUD[index].GetComponent<Image>().enabled = true;

        if (item.IsUtility)
            currentUtilityIndex = index;
        else
            currentOffensiveIndex = index;

    }

    /// <summary>
    /// An item is removed from the player's inventory
    /// </summary>
    /// <param name="index">item to be removed</param>
    public void DisableItem(Item item) {
        var index = item.index;
        var itemArray = item.IsUtility ? utilityItems : offensiveItems;
        var itemHUD = item.IsUtility ? activeUtilityItemHUD : activeOffensiveItemHUD;

        if (itemArray[index].GetComponent<Item>().IsObtained) {
            itemArray[index].GetComponent<Item>().IsObtained = false;
            itemArray[index].GetComponent<Item>().InHand = false;
            itemHUD[index].GetComponent<Image>().enabled = false;
            if (item.IsUtility && currentUtilityIndex == index || !item.IsUtility && currentOffensiveIndex == index) {
                CycleItem(item);
            }
        }
    }

    /// <summary>
    /// An item is added to the player's inventory
    /// </summary>
    /// <param name="index">item to be enabled</param>
    public void EnableItem(Item item) {
        var index = item.index;
        var itemArray = item.IsUtility ? utilityItems : offensiveItems;
        var itemHUD = item.IsUtility ? activeUtilityItemHUD : activeOffensiveItemHUD;

        if (!itemArray[index].GetComponent<Item>().IsObtained) {
            itemArray[currentOffensiveIndex].GetComponent<Item>().InHand = false;
            itemHUD[currentOffensiveIndex].GetComponent<Image>().enabled = false;

            itemArray[index].GetComponent<Item>().IsObtained = true;
            itemArray[index].GetComponent<Item>().InHand = true;
            itemHUD[index].GetComponent<Image>().enabled = true;

            if (item.IsUtility)
                currentUtilityIndex = index;
            else
                currentOffensiveIndex = index;

        }
    }
}
