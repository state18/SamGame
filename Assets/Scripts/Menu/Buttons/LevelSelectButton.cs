using UnityEngine;
using System.Collections;

public class LevelSelectButton : MenuButton {
    MainMenuScreen menuManager;
    ChooseLevel chooseLevel;
    // Use this for initialization
    void Start() {
        menuManager = FindObjectOfType<MainMenuScreen>();
        chooseLevel = FindObjectOfType<ChooseLevel>();
    }

    // Update is called once per frame
    void Update() {

    }

    public override void OnSelectButton() {
        StartCoroutine(MenuManager.Instance.ChangeScreen("ChooseLevel"));
    }

}
