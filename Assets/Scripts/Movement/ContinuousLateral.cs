using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ContinuousLateral : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5.0f;
    public SteamVR_Action_Vector2 action;
    public SteamVR_Input_Sources handType;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = new Vector3()
        {
            x = moveSpeed * action.GetAxis(handType).x * Time.fixedDeltaTime * player.right.x,
            y = 0,
            z = moveSpeed * action.GetAxis(handType).y * Time.fixedDeltaTime * player.forward.z
        };
        player.position += velocity;
    }
}
