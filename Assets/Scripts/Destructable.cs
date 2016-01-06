using UnityEngine;
using System.Collections;

/// <summary>
/// Any GameObject with this component can be destroyed if needed.
/// Example: A bomb destroying a cracked wall
/// </summary>
public class Destructable : MonoBehaviour {

    public GameObject Debris;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    /// <summary>
    /// Destroys this GameObject and executes protocol for its destruction.
    /// </summary>
    public void Destroy() {
        if (Debris != null)
            Instantiate(Debris, transform.position + new Vector3(0f, .5f, 0f), transform.rotation);
        Destroy(gameObject);
    }
}
