using System.Collections;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public static MenuManager Instance;
    [SerializeField]
    private MenuScreen[] screens;
    private int activeScreenIndex;

    void Awake () {
        Instance = this;
    }

    void Start () {
        if (screens == null)
            Debug.Log("forgot to initialize screens array.");

        activeScreenIndex = 0;

    }

    public IEnumerator ChangeScreen(string destination) {

        var prev = screens[activeScreenIndex].GetType().ToString();
        screens[activeScreenIndex].LeavingScreen();
        screens[activeScreenIndex].enabled = false;
        switch (destination) {
            case "MainMenuScreen":
                activeScreenIndex = 0;
                break;
            case "ChooseLevel":
                activeScreenIndex = 1;
                break;

            default:
                throw new System.ArgumentException("Invalid destination", destination);
        }
        yield return StartCoroutine(CameraMoveToLocation.Instance.AnimateMovement(prev + "To" + destination));
        screens[activeScreenIndex].enabled = true;
        screens[activeScreenIndex].EnteringScreen();
    }
}