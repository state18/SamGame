using UnityEngine;
using System.Collections;

public abstract class State {

    public abstract void Entry();
    public abstract void Action();
    public abstract void Exit();
    public abstract State TransitionCheck();
}
