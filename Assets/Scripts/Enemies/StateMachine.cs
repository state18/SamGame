using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine  // does not inherit from Monobehavior
{
	public Stack<Func<IEnumerator>> states;			//stack of states, for prioritization
	public Coroutine runningCoroutine;				//the Coroutine currently running is saved so it can be passed as a parameter to the myStopCoroutine method.
	
	// Use this for initialization
	
	public StateMachine (Func<IEnumerator> initialState)
	{
		states = new Stack<Func<IEnumerator>> ();
		pushState (initialState);
		
	}

	public Func<IEnumerator> popState ()				//Remove the state at the top of the stack and stop its coroutine.
	{
		CoroutineBridge.instance.myStopCoroutine (runningCoroutine);  
		Func<IEnumerator> poppedState = states.Pop ();

		if (getCurrentState () != null) {
            Debug.Log("This should never run");
			runningCoroutine = CoroutineBridge.instance.myStartCoroutine (getCurrentState () ());
		}
		return poppedState;
	}
	
	public void pushState (Func<IEnumerator> state)	//Add state to the stack, and start its coroutine, after stopping the current one's coroutine.
	{
		if (getCurrentState () != state) {

			if (getCurrentState () != null) {
                Debug.Log("this should also never run yet.");
				CoroutineBridge.instance.myStopCoroutine (runningCoroutine);  
			}

			states.Push (state);
			runningCoroutine = CoroutineBridge.instance.myStartCoroutine (getCurrentState () ());     //Start the coroutine of the pushed state
			//Debug.Log ("state pushed");
		}
	}
	
	public Func<IEnumerator> getCurrentState ()
	{
		if (states.Count < 1) {
			//Debug.Log ("stack is empty");
			return null;
		}
		//Debug.Log ("stack peeked");
		
		return states.Peek ();
	}
	
}
