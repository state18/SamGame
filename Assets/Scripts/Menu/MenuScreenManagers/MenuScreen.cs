using UnityEngine;

public abstract class MenuScreen : MonoBehaviour {
    protected bool axisInUse;

    protected abstract void Update();
    public virtual void EnteringScreen() {
        axisInUse = true;
    }
    public virtual void LeavingScreen() { }
}