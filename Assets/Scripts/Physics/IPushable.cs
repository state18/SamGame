using UnityEngine;

interface IPushable {
    void PushHorizontal(float push);
    void PushVertical(float push, GameObject instigator);
}

