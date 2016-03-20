using UnityEngine;
using System.Collections;

public class FlyingBotFSM : FiniteStateMachine {

	// Use this for initialization
	protected override void Start () {
        currentState = new FlyingBotPatrolState();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}
}
