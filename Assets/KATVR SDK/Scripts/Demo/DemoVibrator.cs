using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class DemoVibrator : MonoBehaviour
{
    
    void Start()
    {
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener((fromAction,fromSource) => 
        {
            KATVibrator.Haptic_Module_Control(5, 1000);
        }, 
        SteamVR_Input_Sources.Any);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.K))
        {
            KATVibrator.Haptic_Module_Control(5, 1000);
        }
#endif
    }
}
