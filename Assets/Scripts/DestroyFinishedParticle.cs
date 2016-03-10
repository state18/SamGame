using UnityEngine;
using System.Collections;

public class DestroyFinishedParticle : MonoBehaviour {

    private ParticleSystem thisParticleSystem;

    // Use this for initialization
    void Start() {
        thisParticleSystem = GetComponent<ParticleSystem>();
        StartCoroutine("AwaitDestruction");
    }

    IEnumerator AwaitDestruction() {
        yield return new WaitForSeconds(thisParticleSystem.duration);
        Destroy(gameObject);
    }
}

// The following code is code that should work but is currently bugged in Unity. Awaiting a fix.
/*
using UnityEngine;
using System.Collections;

public class DestroyFinishedParticle : MonoBehaviour
{

	private ParticleSystem thisParticleSystem;

	// Use this for initialization
	void Start ()
	{
		thisParticleSystem = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (!thisParticleSystem.IsAlive())
            Destroy(gameObject);
	}
}
*/
