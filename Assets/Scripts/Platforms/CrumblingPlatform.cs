using UnityEngine;
using System.Collections;
using System;

public class CrumblingPlatform : MonoBehaviour {
    [SerializeField]
    private float crumbleIn;
    [SerializeField]
    private float comeBackIn;

    private bool isCrumbling = false;
    private SpriteRenderer[] sr;
    private EdgeCollider2D edgeCollider;

    void Start() {
        sr = GetComponentsInChildren<SpriteRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void ControllerEnter2D(CharacterController2D controller) {
        if (controller.GetComponent<Player>() && !isCrumbling)
            StartCoroutine("Crumble");

        Debug.Log("entered platform");
    }

    public void ControllerExit2D() {
        Debug.Log("leaving platform");
    }

    private IEnumerator Crumble() {

        //  Start crumbling animation here.
        isCrumbling = true;
        yield return StartCoroutine(CoroutineBridge.instance.Wait(crumbleIn));
        edgeCollider.enabled = false;
        for (int i = 0; i < sr.Length; i++) {
            sr[i].enabled = false;
        }

        yield return StartCoroutine(CoroutineBridge.instance.Wait(comeBackIn));

        edgeCollider.enabled = true;
        for (int i = 0; i < sr.Length; i++) {
            sr[i].enabled = true;
        }
        isCrumbling = false;
    }
}
