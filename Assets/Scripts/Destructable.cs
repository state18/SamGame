using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

    public GameObject wallDebris;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void DestroyMe() {
        if (wallDebris != null)
            Instantiate(wallDebris, transform.position + new Vector3(0f, .5f, 0f), transform.rotation);
        Destroy(gameObject);
    }
}
