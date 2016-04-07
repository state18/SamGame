using UnityEngine;

public abstract class MenuScreen : MonoBehaviour {
    protected abstract void Update();
    public virtual void EnteringScreen() { }
    public virtual void LeavingScreen() { }
}