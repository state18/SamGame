using UnityEngine;
using System.Collections;

/// <summary>
/// Facilitates camera animations that manipulate its transform component.
/// </summary>
public class CameraMoveToLocation : MonoBehaviour {

    public static CameraMoveToLocation Instance;
    Animator anim;
    bool isMoving = false;

    // Use this for initialization
    void Start() {
        Instance = this;
        anim = GetComponent<Animator>();

    }

    public IEnumerator AnimateMovement(string triggerName) {
        isMoving = true;
        anim.SetTrigger(triggerName);

        while (isMoving)
            yield return null;
    }

    void AnimationComplete() {
        isMoving = false;
    }
}
