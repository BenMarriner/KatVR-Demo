using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InventoryUI : MonoBehaviour
{
    GameObject canvas;

    public SteamVR_Action_Boolean action;
    public SteamVR_Input_Sources handType;
    public bool visible;

    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (action.GetStateDown(handType)) visible = !visible;
        canvas.SetActive(visible);
    }

    private void OnValidate()
    {
        canvas = transform.Find("Canvas").gameObject;
        canvas.SetActive(visible);
    }
}
