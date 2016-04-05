using UnityEngine;
using System.Collections;

public class QuitGameButton : MenuButton {

    public override void OnSelectButton() {
        Application.Quit();
    }
}
