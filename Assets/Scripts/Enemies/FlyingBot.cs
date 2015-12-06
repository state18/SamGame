using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FlyingBot : MonoBehaviour {
    //Patrol editable fields

    public PathDefinition patrolPath;       //Where will this entity patrol?
    public int startingPoint;           //From which node will the entity begin patrolling?
    public float targetedVelocity;      //How fast does the entity move by default?
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

    StateMachine brain;

    BoxCollider2D myCollider;

    Transform playerTransform;
    CharacterController2D playerController;

    private IEnumerator<Transform> currentPoint;

    // Use this for initialization
    void Start() {
        playerTransform = FindObjectOfType<Player>().transform;
        playerController = playerTransform.GetComponent<CharacterController2D>();
        myCollider = GetComponent<BoxCollider2D>();

        //AI initialized, and Patrol state is set
        brain = new StateMachine(PatrolState);

        //brain.pushState (Patrol);


        if (patrolPath == null) {
            Debug.LogError("Path cannot be null", gameObject);
            return;
        }

        currentPoint = patrolPath.GetPathEnumerator();
        for (int i = 0; i <= startingPoint; i++)
            currentPoint.MoveNext();


        if (currentPoint.Current == null)
            return;

        transform.position = currentPoint.Current.position;
    }


    public IEnumerator PatrolState()    //Move around a contained area
    {
        Debug.Log("Patrolling");

        #region Patrol Entry
        yield return null;
        #endregion

        while (true) {

            Debug.Log("Patrolling main loop.");
            //Enemy moves towards the next point in its path. If at destination, cycle to the next point
            #region Patrol Action

            float maxDistanceToGoal = .1f;

            if (currentPoint == null || currentPoint.Current == null) {
                Debug.Log("currentPoint is null");
                yield return null;
            }
            var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
            if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal) {                              //Has the entity arrived at the destination node?

                currentPoint.MoveNext();

            }
            transform.position = Vector2.MoveTowards(transform.position, currentPoint.Current.position, Time.deltaTime * targetedVelocity);
            #endregion

            //Transition Checks: MoveToPlayer
            #region To MoveToPlayer
            if (Physics2D.OverlapCircle(transform.position, detectionRadius, whatIsPlayer) && botBounds.Contains(playerTransform.position)) { //TODO CircleRaycast so it doesn't see player through walls, also  don't call this every frame, use coroutine
                Debug.Log("From Patrol to MoveToPlayer");
                brain.popState();
                brain.pushState(MoveToPlayerState);

                yield return null;
            }
            #endregion

            yield return null;
        }
    }

    public IEnumerator MoveToPlayerState()  //Fly over the player while they are in range of the entity AND within the entity's max bounds
    {
        Debug.Log("Moving To Player");

        #region MoveToPlayer Entry
        yield return null;
        #endregion

        while (true) {

            #region MoveToPlayer Action
            Debug.Log("MoveToPlayer main loop");
            float maxDistanceToGoal = .3f;                                      // How close does the bot need to be to be considered at the destination?
            float step = playerController.Velocity.magnitude * 1.2f;            //the entity will move a little bit faster than the player is moving

            if (step < targetedVelocity)                                               //If adjusted speed is slower than regular speed, move at regular speed.
                step = targetedVelocity;

            Vector3 actualDestination = new Vector3(0, yOffsetFromPlayer, 0) + playerTransform.position;        //the point where the entity wants to go
            transform.position = Vector2.MoveTowards(transform.position, actualDestination, step * Time.deltaTime);
            #endregion

            //Transition Checks: SlamDownwards, Patrol

           
            #region To Patrol
                if (!botBounds.Contains(playerTransform.position)) {
                Debug.Log("movetoplayer to patrolling");
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
                Debug.Log("From MoveToPlayer to SlamDownwards");
                //isPreSlamDownwardsRunning = true;
                //StartCoroutine("PreSlamDownwardsCoroutine");                //begins a timer that switches to slam state when it expires, but only if player is still being followed!

                brain.popState();
                brain.pushState(SlamDownwardsState);

                yield return null;

            }


            #endregion

            yield return null;
        }
    }


    public IEnumerator SlamDownwardsState() // After a brief delay, slam downwards (this is the window for the player to kill this enemy)
    {
        #region SlamDownwards Entry
        yield return null;
        //TODO Speed up the animation of the bot, and give its color a different tint to let the player know it is about to slam.
        //After a very short delay, the bot will descend a certain length or until it touches the ground. It will remain on the ground for a short while.
        //This is where the player will be able to attack. Afterwards, transition it back to Patrol State.
        Animator botAnim = GetComponent<Animator>();                                                        //The bot changes to a red tint and doubles its animation speed
        SpriteRenderer botSprite = GetComponent<SpriteRenderer>();
        botAnim.speed = 2;
        botSprite.color = Color.red;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 10f, whatisGround);      //stores info about the ground hit by raycasting
        Vector3 distanceWithOffset = new Vector3(0f, myCollider.size.y / 2, 0f) + new Vector3(hit.point.x, hit.point.y, 0f);                                                                        //this offsets the ray to the bottom of the bot
        //float distanceToSlam = hit.distance - distanceOffset;
        //Debug.Log("Distance to slam: " + distanceToSlam);

        Debug.Log("Waiting to turn white again.");

        //yield return CoroutineBridge.instance.myStartCoroutine(CoroutineBridge.instance.Wait(delayUntilSlam));
        yield return new WaitForSeconds(delayUntilSlam);

        Debug.Log("Changing back to white.");
        botAnim.speed = 1;
        botSprite.color = Color.white;

        #endregion
        while (true) {

            #region SlamDownwards Action
            //TODO fill me up with slamming code.
            float step = targetedVelocity * 1.5f;
            // Initialized at placeholder one.
            // TODO IMPORTANT RECENT WORK: Refer to the FollowPath script to see how to detect when an object using MoveTowards has reached its destination!!!!
            

            if (hit.collider != null) {
                Debug.Log("Moving towards hit object!");
                transform.position = Vector2.MoveTowards(transform.position, distanceWithOffset, step * Time.deltaTime);              
            }

            var distanceSquared = (transform.position - distanceWithOffset).sqrMagnitude;

            #endregion

            #region ToPatrol
            
            if (distanceSquared < .01f) {
                Debug.Log("Going back to Patrol state from Slam State.");
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
