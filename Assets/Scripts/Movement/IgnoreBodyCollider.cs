using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class IgnoreBodyCollider : MonoBehaviour
{
    Collider bodyCollider;
    HandCollider handCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!bodyCollider) bodyCollider = transform.root.Find("BodyCollider").GetComponent<Collider>();
        handCollider = GetComponent<HandCollider>();
        foreach (var fingerCollider in handCollider.Colliders) { Physics.IgnoreCollision(bodyCollider, fingerCollider); }
    }
}
