using UnityEngine;
using System.Collections;

public abstract class FiniteStateMachine : MonoBehaviour {

    protected State currentState;

    /// <summary>
    /// Derived classes will use Start() to initalize the currentState and anything else needed.
    /// </summary>
    protected abstract void Start();

    /// <summary>
    /// Derived classes should call this every frame.
    /// </summary>
    protected virtual void Update() {
        
    }
}
