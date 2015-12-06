using UnityEngine;
using System.Collections;

public class Bombable : MonoBehaviour {

    public GameObject Debris;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnBombed() {
        if (Debris != null)
            Instantiate(Debris, transform.position + new Vector3(0f, .5f, 0f), transform.rotation);
        Destroy(gameObject);
    }
}
