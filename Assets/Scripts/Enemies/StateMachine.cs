using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Maintains a stack of "states", and keeps the top element as a running coroutine, with the rest being history.
/// </summary>
public class StateMachine  // does not inherit from Monobehavior
{
    public Stack<Func<IEnumerator>> states;         //stack of states, for prioritization
    public Coroutine runningCoroutine;              //the Coroutine currently running is saved so it can be passed as a parameter to the myStopCoroutine method.
    public MonoBehaviour entity;

    // Use this for initialization

    /// <summary>
    /// Constructor that needs to know which Monobehaviour the states are coming from, and a starting state.
    /// </summary>
    /// <param name="initialState"></param>
    /// <param name="entity"></param>
    public StateMachine(Func<IEnumerator> initialState, MonoBehaviour entity) {
        states = new Stack<Func<IEnumerator>>();
        this.entity = entity;
        pushState(initialState);


    }

    /// <summary>
    /// Removes the currently running state from the stack and activates the new top state.
    /// </summary>
    /// <returns></returns>
    public Func<IEnumerator> popState()             //Remove the state at the top of the stack and stop its coroutine.
    {
        entity.StopCoroutine(runningCoroutine);
        //CoroutineBridge.instance.myStopCoroutine (runningCoroutine);  
        Func<IEnumerator> poppedState = states.Pop();

        if (getCurrentState() != null) {
            runningCoroutine = entity.StartCoroutine(getCurrentState()());
            //runningCoroutine = CoroutineBridge.instance.myStartCoroutine (getCurrentState () ());
        }
        return poppedState;
    }

    /// <summary>
    /// Stops the current state from running and puts on a new one to be run.
    /// </summary>
    /// <param name="state"></param>
    public void pushState(Func<IEnumerator> state)  //Add state to the stack, and start its coroutine, after stopping the current one's coroutine.
    {
        if (getCurrentState() != state) {

            if (getCurrentState() != null) {
                entity.StopCoroutine(runningCoroutine);
                //CoroutineBridge.instance.myStopCoroutine (runningCoroutine);  
            }

            states.Push(state);
            runningCoroutine = entity.StartCoroutine(getCurrentState()());
            //runningCoroutine = CoroutineBridge.instance.myStartCoroutine (getCurrentState () ());     //Start the coroutine of the pushed state
            //Debug.Log("state pushed");
        }
    }

    /// <summary>
    /// Stops running the current state and clear the history.
    /// </summary>
    public void clearStates() {
        entity.StopCoroutine(runningCoroutine);
        states.Clear();
    }

    /// <summary>
    /// Retrieves the running state
    /// </summary>
    /// <returns></returns>
    public Func<IEnumerator> getCurrentState() {
        if (states.Count < 1) {
            //Debug.Log ("stack is empty");
            return null;
        }
        //Debug.Log ("stack peeked");

        return states.Peek();
    }

}
