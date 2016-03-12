using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to store GameObjects to be reused instead of instantiating/destroying them each time they're needed.
/// </summary>
public class ObjectPooler : MonoBehaviour {

    public GameObject pooledObject;
    public int pooledAmount = 10;
    public bool willGrow = true;

    List<GameObject> pooledObjects;

    void Start() {
        if (pooledObject == null)
            throw new System.InvalidOperationException("Object Pooler not initialized!");

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++) {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public void Initialize (GameObject pooledObject, int pooledAmount, bool willGrow) {
        this.pooledObject = pooledObject;
        this.pooledAmount = pooledAmount;
        this.willGrow = willGrow;
    }

    public GameObject GetPooledObject() {

        for(int i = 0; i < pooledObjects.Count; i++) {
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        }

        if (willGrow) {
            GameObject obj = Instantiate(pooledObject);
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }
}
