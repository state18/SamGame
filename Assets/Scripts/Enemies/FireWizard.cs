using System;
using UnityEngine;

public class FireWizard : MonoBehaviour, ITakeDamage, IRespawnable{
    

    void Start() {

    }

    void Update() {

    }

    public void RespawnMe() {
        throw new NotImplementedException();
    }

    public void TakeDamage(int damage, GameObject instigator) {
        throw new NotImplementedException();
    }
}