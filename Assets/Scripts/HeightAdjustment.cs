using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightAdjustment : MonoBehaviour
{
    public float playerHeight = 1.7f;
    public float playerRadius = 0.5f;
    private CapsuleCollider playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        playerCollider.center = new Vector3()
        {
            x = 0,
            y = playerHeight / 2,
            z = 0
        };

        playerCollider.height = playerHeight;
        playerCollider.radius = playerRadius;
    }
}
