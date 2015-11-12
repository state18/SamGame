﻿using UnityEngine;
using System.Collections;

public class HandItem : Item
{
	
	public Transform handStart;
	public Transform handFinish;
	private CharacterController2D player;
	
	Animator playerAnim;
	
	public float distance;
	private float cooldown = 0f;
	public float cooldownTime;
	public float enemyKnockback;
	//private int rayAmount = 6;
	private int ongoingPunches = 0;
	public int damageToGive;
	//public float radius;
	public Vector2 boxSize;
	public int hitDirection;
	
	public LayerMask punchable;
	
	public HandItem ()
	{
		InHand = false;
		IsObtained = true;
	}
	// Use this for initialization
	void Start ()
	{
		playerAnim = handStart.GetComponentInParent<Animator> ();
		player = handStart.GetComponentInParent<CharacterController2D> ();
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
		
        // Remember to handle climbing state!!
		if (InHand && Input.GetKeyDown (KeyCode.X)) {
			Use ();
		}
	}
	
	public void Use ()
	{
		if (cooldown <= 0) {
			GetComponent<AudioSource> ().Play ();
			StartCoroutine ("UseCo");
			
		}
	}
	public IEnumerator UseCo ()
	{
		RaycastHit2D[] hit;
		//bool hitSomething = false;
		cooldown = cooldownTime;
		ongoingPunches++;
		playerAnim.SetBool ("isPunching", true);
		
		float adjustedDistance = (Mathf.Abs(player.Velocity.x) > 0) ? distance + .33f : distance;
		
		//Debug.Log (adjustedDistance);
		
		
		Vector2 origin = Vector2.Lerp (new Vector2 (handStart.position.x, handStart.position.y), new Vector2 (handFinish.position.x, handFinish.position.y), .5f);
		
		if (handStart.parent.transform.localScale.x > 0) {
			
			
			hit = Physics2D.BoxCastAll (origin, boxSize, 0f, Vector2.right, adjustedDistance, punchable);
			
			hitDirection = 1;
			
			
			
		} else {
			
			//hit = Physics2D.CircleCastAll (origin, radius, -Vector2.right, distance, punchable);
			hit = Physics2D.BoxCastAll (origin, boxSize, 0f, -Vector2.right, adjustedDistance, punchable);
			hitDirection = 0;
			
		}
		//TODO  knockback the enemy (add another parameter to giveDamage method on EnemyHealthManager maybe?)
		
		
		foreach (RaycastHit2D j in hit) {
			
			if ((j.collider != null) && (j.collider.tag == "Enemy") && j.collider.isTrigger) {
				Debug.Log ("hit enemy");
                ((ITakeDamage)j.collider.GetComponent(typeof(ITakeDamage))).TakeDamage(damageToGive, gameObject);
				
				
			} else if (j.collider != null && j.collider.tag == "Lever") {
				var leverControl = j.collider.GetComponent<LeverBehavior> ();
				if (!leverControl.isOn) {
					leverControl.TellDoorOpen ();
				} else if (leverControl.isOn)
					leverControl.TellDoorClose ();
			}
		}
		
		
		yield return new WaitForSeconds (.18f);
		
		ongoingPunches--;
		if (ongoingPunches <= 0)
			playerAnim.SetBool ("isPunching", false);
		
	}
}