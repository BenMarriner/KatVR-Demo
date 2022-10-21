using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class IgnoreBodyCollider : MonoBehaviour
{
    public Collider bodyCollider;
    public HandCollider handCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!bodyCollider) handCollider = GetComponent<HandCollider>();
        foreach (var fingerCollider in handCollider.Colliders) { Physics.IgnoreCollision(bodyCollider, fingerCollider); }
    }
}
