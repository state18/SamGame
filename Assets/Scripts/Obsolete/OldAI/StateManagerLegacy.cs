using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateManagerLegacy  // does not inherit from Monobehavior
{

	public Stack<Action> states;			//stack of states, for prioritization

	// Use this for initialization

	public StateManagerLegacy (Action initialState)
	{
		states = new Stack<Action> ();

		pushState (initialState);

	}
	

	public void ManualUpdate ()		//to distinguish itself from a Monobehavior's Update function
	{

		if (getCurrentState () != null) {		//if currently in a state, invoke the state's function
			//Debug.Log ("current state executed");
			getCurrentState () ();		//invokes the Func currentState then invokes the Action it returns
		}
	}

	public Action popState ()
	{
		return states.Pop ();
	}

	public void pushState (Action state)
	{
		if (getCurrentState () != state) {
			states.Push (state);
			Debug.Log ("state pushed");
		}
	}

	public Action getCurrentState ()
	{
		if (states.Count < 1) {
			Debug.Log ("stack is empty");
			return null;
		}
		//Debug.Log ("stack peeked");

		return states.Peek ();
	}

}
