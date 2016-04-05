using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
    //Singleton pattern enforced through this reference
    public static MenuManager Instance { get; private set; }

    public static GameObject[] menuButtons;
    public static GameObject[] menuArrows;

    public GameObject newGame;
    public GameObject levelSelect;
    public GameObject quitGame;

    public GameObject newGameArrow;
    public GameObject levelSelectArrow;
    public GameObject quitGameArrow;



    public static int currentIndex;
    public static int numButtons = 3;

    bool axisInUse;
    public bool movementLocked;

    void Awake() {
        //Are there any other instances of this class?
        if (Instance != null && Instance != this) {
            //This instance is useless, destroy it
            Destroy(gameObject);
        }
        //Saves this object as our singleton instance
        Instance = this;
    }
    // Use this for initialization
    void Start() {

        menuButtons = new GameObject[numButtons];
        menuArrows = new GameObject[numButtons];
        currentIndex = 0;

        menuButtons[0] = newGame;
        menuButtons[1] = levelSelect;
        menuButtons[2] = quitGame;

        menuArrows[0] = newGameArrow;
        menuArrows[1] = levelSelectArrow;
        menuArrows[2] = quitGameArrow;

        menuButtons[0].GetComponent<MenuButton>().IsUnlocked = true;
        menuButtons[0].GetComponent<MenuButton>().IsSelected = true;
        menuArrows[0].GetComponent<SpriteRenderer>().enabled = true;

        menuButtons[1].GetComponent<MenuButton>().IsUnlocked = true;
        menuButtons[2].GetComponent<MenuButton>().IsUnlocked = true;

        //fill array here with menu button objects
    }

    // Update is called once per frame
    void Update() {
        if (!movementLocked) {
            if (Input.GetAxisRaw("Vertical") == -1) {
                if (!axisInUse) {
                    Cycle(-1);

                    axisInUse = true;
                }

            } else if (Input.GetAxisRaw("Vertical") == 1) {
                if (!axisInUse) {
                    Cycle(1);

                    axisInUse = true;
                }

            } else if (Input.GetAxisRaw("Vertical") == 0) {
                axisInUse = false;
            }

            if (Input.GetButtonDown("Jump")) {

                menuButtons[currentIndex].GetComponent<MenuButton>().OnSelectButton();
            }

        }
    }

    public void Cycle(int direction) {
        menuButtons[currentIndex].GetComponent<MenuButton>().IsSelected = false;
        menuArrows[currentIndex].GetComponent<SpriteRenderer>().enabled = false;

        do {

            currentIndex -= direction;


            if (currentIndex < 0) {
                currentIndex = menuButtons.Length - 1;




            } else if (currentIndex > menuButtons.Length - 1) {
                currentIndex = 0;



            }



        } while (!menuButtons[currentIndex].GetComponent<MenuButton>().IsUnlocked);

        menuButtons[currentIndex].GetComponent<MenuButton>().IsSelected = true;
        menuArrows[currentIndex].GetComponent<SpriteRenderer>().enabled = true;

        Debug.Log("Current index:" + currentIndex);
    }

    public static void LeavingScreen() {
        Instance.movementLocked = true;
    }

    public static void EnteringScreen() {
        Instance.movementLocked = false;
    }
}