﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Holds the states used by the FlyingBot. They are passed into an instance of StateMachine to be managed as coroutines.
/// </summary>
public class FlyingBot : Enemy {

    //Patrol editable fields

    public PathDefinition patrolPath;       //Where will this entity patrol?
    public int startingPoint;           //From which node will the entity begin patrolling?
    public float detectionRadius;       //How far away can the entity see the player?
    public LayerMask whatIsPlayer;
    public Rect botBounds;              //How far away can the entity follow the player?

    //MoveToPlayer editable fields

    public float yOffsetFromPlayer;                 //distance between the player and the target point of the entity (entity targets a point around player, not the player directly)
    public float delayUntilSlam;                    //how long will the entity wait until it slams after reaching the player?
                                                    //MoveToPlayer coroutine flag check
                                                    //bool isPreSlamDownwardsRunning = false;         //The coroutine should not be executed repeatedly while it is already running

    //SlamDownwards editable fields
    public LayerMask whatisGround;
    [SerializeField]
    private float slamRaycastDistance;

    StateMachine brain;

    BoxCollider2D myCollider;

    Transform playerTransform;
    CharacterController2D playerController;

    private IEnumerator<Transform> currentPoint;
    private bool hasBeenInitialized = false;

    // Use this for initialization
    void Start() {
        Initialize();
        playerTransform = FindObjectOfType<Player>().transform;
        playerController = playerTransform.GetComponent<CharacterController2D>();

        myCollider = GetComponent<BoxCollider2D>();
        Health = MaxHealth;

        //AI initialized, and Patrol state is set
        brain = new StateMachine(PatrolState, this);

        //brain.pushState (Patrol);


        if (patrolPath == null) {
            //Debug.LogError("Path cannot be null", gameObject);
            return;
        }

        currentPoint = patrolPath.GetPathEnumerator();
        for (int i = 0; i <= startingPoint; i++)
            currentPoint.MoveNext();


        if (currentPoint.Current == null)
            return;

        transform.position = currentPoint.Current.position;
        startPosition = transform.position;
        hasBeenInitialized = true;
    }

    void OnEnable() {
        if (hasBeenInitialized && brain.states.Count == 0)
            brain.pushState(PatrolState);
    }

    void OnDisable() {
        // Clear the state machine's history and reset possible altered attributes of this entity back to the default settings.
        brain.clearStates();
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Animator>().speed = 1;
    }

    /// <summary>
    /// Follow a path going from point to point.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PatrolState()    //Move around a contained area
    {
        //Debug.Log("Patrolling");

        #region Patrol Entry
        yield return null;
        #endregion

        while (true) {

            //Debug.Log("Patrolling main loop.");
            //Enemy moves towards the next point in its path. If at destination, cycle to the next point
            #region Patrol Action

            float maxDistanceToGoal = .1f;

            if (currentPoint == null || currentPoint.Current == null) {
                //Debug.Log("currentPoint is null");
                yield return null;
            }
            var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
            if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {                              //Has the entity arrived at the destination node?

                currentPoint.MoveNext();

            }
            transform.position = Vector2.MoveTowards(transform.position, currentPoint.Current.position, Time.deltaTime * Speed);
            #endregion

            //Transition Checks: MoveToPlayer
            #region To MoveToPlayer
            if (Physics2D.OverlapCircle(transform.position, detectionRadius, whatIsPlayer) && botBounds.Contains(playerTransform.position)) { //TODO CircleRaycast so it doesn't see player through walls, also  don't call this every frame, use coroutine
                //Debug.Log("From Patrol to MoveToPlayer");
                brain.popState();
                brain.pushState(MoveToPlayerState);

                yield return null;
            }
            #endregion

            yield return null;
        }
    }

    /// <summary>
    /// Move towards a point offset vertically from the player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveToPlayerState()  //Fly over the player while they are in range of the entity AND within the entity's max bounds
    {
        //Debug.Log("Moving To Player");

        #region MoveToPlayer Entry
        yield return null;
        #endregion

        while (true) {

            #region MoveToPlayer Action
            //Debug.Log("MoveToPlayer main loop");
            float maxDistanceToGoal = .3f;                                      // How close does the bot need to be to be considered at the destination?
            float step = playerController.Velocity.magnitude * 1.2f;            //the entity will move a little bit faster than the player is moving

            if (step < Speed)                                               //If adjusted speed is slower than regular speed, move at regular speed.
                step = Speed;

            Vector3 actualDestination = new Vector3(0, yOffsetFromPlayer, 0) + playerTransform.position;        //the point where the entity wants to go
            transform.position = Vector2.MoveTowards(transform.position, actualDestination, step * Time.deltaTime);
            #endregion

            //Transition Checks: SlamDownwards, Patrol


            #region To Patrol
            if (!botBounds.Contains(playerTransform.position)) {
                //Debug.Log("movetoplayer to patrolling");
                //StopCoroutine("PreSlamDownwardsCoroutine");
                //isPreSlamDownwardsRunning = false;
                brain.popState();
                brain.pushState(PatrolState);

                yield return null;
            }

            #endregion

            #region To SlamDownwards
            var distanceSquared = (transform.position - actualDestination).sqrMagnitude;

            if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {                    //entity has reached the targeted location 
                //Debug.Log("From MoveToPlayer to SlamDownwards");
                brain.popState();
                brain.pushState(SlamDownwardsState);

                yield return null;

            }


            #endregion

            yield return null;
        }
    }


    /// <summary>
    /// After a delay, move downwards until hitting ground, then transition to PatrolState.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SlamDownwardsState() // After a brief delay, slam downwards (this is the window for the player to kill this enemy)
    {
        #region SlamDownwards Entry
        yield return null;
        Animator botAnim = GetComponent<Animator>();                                                        //The bot changes to a red tint and doubles its animation speed
        SpriteRenderer botSprite = GetComponent<SpriteRenderer>();
        botAnim.speed = 2;
        botSprite.color = Color.red;

        var leftRayOrigin = myCollider.bounds.center - new Vector3(myCollider.bounds.extents.x, myCollider.bounds.extents.y);

        var rayCount = Mathf.CeilToInt(myCollider.bounds.size.x / .2f + 1);
        var actualDistanceBetweenRays = myCollider.bounds.size.x / (rayCount - 1);

        // Initializing distanceToSlam to -1 is useful later on, because if it remains -1, we know no relevant point has been hit by the raycasts.
        float distanceToSlam = slamRaycastDistance;
        for (int i = 0; i < rayCount; i++) {
            var rayVector = new Vector2(leftRayOrigin.x + (i * actualDistanceBetweenRays), leftRayOrigin.y);
            var raycastHit = Physics2D.RaycastAll(rayVector, Vector2.down, slamRaycastDistance, whatisGround);
            Debug.DrawRay(rayVector, Vector2.down * slamRaycastDistance, Color.red);

            if (raycastHit.Length == 0)
                continue;

            var relevantHits = raycastHit.Where(x => x.distance > 0f);

            if (relevantHits.Count() == 0)
                continue;

            var minimumDistance = relevantHits.Min(x => x.distance);
            if (minimumDistance < distanceToSlam)
                distanceToSlam = minimumDistance;

        }

        var targetLocation = leftRayOrigin + new Vector3(myCollider.bounds.extents.x, -distanceToSlam + myCollider.bounds.extents.y);

        yield return new WaitForSeconds(delayUntilSlam);

        //Debug.Log("Changing back to white.");
        botAnim.speed = 1;
        botSprite.color = Color.white;

        #endregion
        while (true) {

            #region SlamDownwards Action

            float step = Speed * 1.5f;

            //Debug.Log("Moving towards hit object!");
            transform.position = Vector2.MoveTowards(transform.position, targetLocation, step * Time.deltaTime);

            var distanceSquared = (transform.position - targetLocation).sqrMagnitude;

            #endregion

            #region ToPatrol

            if (distanceSquared < .01f) {
                //Debug.Log("Going back to Patrol state from Slam State.");
                // It is important we stop this as ending a parent coroutine does not end those nested inside of it.

                yield return new WaitForSeconds(delayUntilSlam);
                //yield return StartCoroutine(WaitCo.Wait(delayUntilSlam));
                //yield return CoroutineBridge.instance.myStartCoroutine(CoroutineBridge.instance.Wait(delayUntilSlam));

                brain.popState();
                brain.pushState(PatrolState);

            }
            #endregion

            yield return null;
        }
    }

    // The outline of the botBounds rectangle is drawn in the editor for convenience.
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(botBounds.max, botBounds.min);
    }
}
