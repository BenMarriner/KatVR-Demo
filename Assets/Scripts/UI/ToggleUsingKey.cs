using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUsingKey : MonoBehaviour
{
    public KeyCode key;
    public GameObject toggleObject;
    public bool state = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            state = !state;
            toggleObject.SetActive(state);
        }
    }

    private void OnValidate()
    {
        toggleObject.SetActive(state);
    }
}
