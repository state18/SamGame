using UnityEngine;
using System.Collections;
using UnityEngine.UI;
// Active items can be cycled through. 
// Only one passive item may be held at a time.
// 0 = no item in use (hand)
// 1 = pistol
// 2 = bombs

public class ItemManager : MonoBehaviour {
    public static ItemManager Instance;

    public GameObject pistol;
    public GameObject hand;
    public GameObject bombs;

    public Image handHUD;
    public Image pistolHUD;
    public Image bombHUD;

    private const int size = 3;

    private GameObject[] items;
    private Image[] activeItemHUD;

    private int currentIndex;

    private Player player;

    void Awake() {
        Instance = this;
    }

    // Use this for initialization
    void Start() {

        player = FindObjectOfType<Player>();

        // Hard code the following as more items are added to the game. This structure will never change throughout the game.

        items = new GameObject[size];
        activeItemHUD = new Image[size];
        currentIndex = 0;

        items[0] = hand;
        items[1] = pistol;
        items[2] = bombs;

        activeItemHUD[0] = handHUD;
        activeItemHUD[1] = pistolHUD;
        activeItemHUD[2] = bombHUD;

        // Hand item is initialized. (Player will always have the sword)
        items[0].GetComponent<Item>().IsObtained = true;
        items[0].GetComponent<Item>().InHand = true;
        activeItemHUD[0].GetComponent<Image>().enabled = true;


        //items [1].GetComponent<Item> ().IsObtained = true;
        //items [2].GetComponent<Item> ().IsObtained = true;

    }

    // Update is called once per frame
    void Update() {
        // The only time a player can't swap their items is when they're dead (or in the future, when a cutscene is active)
        if (!player.IsDead) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                Cycle(-1);
            }
            if (Input.GetKeyDown(KeyCode.W)) {
                Cycle(1);
            }
        }

        if (Input.GetButtonDown("Use") && player.CanUseItems)
            items[currentIndex].GetComponent<Item>().Use();


    }
    public void Cycle(int direction) {
        items[currentIndex].GetComponent<Item>().InHand = false;
        activeItemHUD[currentIndex].GetComponent<Image>().enabled = false;

        do {

            currentIndex += direction;

            if (currentIndex < 0) {
                currentIndex = items.Length - 1;

            } else if (currentIndex > items.Length - 1) {
                currentIndex = 0;
            }

        } while (!items[currentIndex].GetComponent<Item>().IsObtained);

        items[currentIndex].GetComponent<Item>().InHand = true;
        activeItemHUD[currentIndex].GetComponent<Image>().enabled = true;

        Debug.Log("Current index:" + currentIndex);
    }


    public void DisableItem(int index) {
        if (items[index].GetComponent<Item>().IsObtained) {
            items[index].GetComponent<Item>().IsObtained = false;
            items[index].GetComponent<Item>().InHand = false;
            activeItemHUD[index].GetComponent<Image>().enabled = false;
            if (currentIndex == index) {
                Cycle(1);
            }
        }
    }

    public void EnableItem(int index) {
        if (!items[index].GetComponent<Item>().IsObtained) {
            items[index].GetComponent<Item>().IsObtained = true;
            items[index].GetComponent<Item>().InHand = true;
            activeItemHUD[index].GetComponent<Image>().enabled = true;

            items[currentIndex].GetComponent<Item>().InHand = false;
            activeItemHUD[currentIndex].GetComponent<Image>().enabled = false;
            currentIndex = index;

        }
    }
}
