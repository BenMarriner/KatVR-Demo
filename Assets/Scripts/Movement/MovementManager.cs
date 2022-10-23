using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using KATVR;
using Valve.VR;

public class MovementManager : MonoBehaviour
{
    public enum LateralMovements { KAT, Teleport, Continuous }
    public enum RotationalMovements { KAT, SnapTurn, Continuous }
    
    // KAT movement (this will override both lateral and rotational movements being used)
    public KATDevice katMovement;
    // Lateral movements
    public Teleport teleportMovement;
    public ContinuousLateral continuousLateralMovement;
    // Rotational movements
    public SnapTurn snapTurn;
    // To do: add continuous rotation

    public LateralMovements currentLateralSetting;
    public RotationalMovements currentRotationalSetting;
    public float currentSnapAngle;
    public float currentTurnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        SetMovements(currentLateralSetting, currentRotationalSetting);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSnapTurnAngle(float angle)
    {
        currentSnapAngle = angle;
        snapTurn.snapAngle = currentSnapAngle;
    }

    public void SetTurnSpeed(float speed)
    {
        currentTurnSpeed = speed;
    }

    public void SetMovements(LateralMovements lateralType, RotationalMovements rotationalType)
    {
        if (lateralType == LateralMovements.KAT || rotationalType == RotationalMovements.KAT)
        {
            currentLateralSetting = LateralMovements.KAT;
            currentRotationalSetting = RotationalMovements.KAT;
        }
        else
        {
            currentLateralSetting = lateralType;
            currentRotationalSetting = rotationalType;
        }

        SetLateralMovement(currentLateralSetting);
        SetRotationalMovement(currentRotationalSetting);
    }

    private void SetLateralMovement(LateralMovements type)
    {
        katMovement.gameObject.SetActive(false);
        teleportMovement.gameObject.SetActive(false);
        continuousLateralMovement.gameObject.SetActive(false);

        switch (type)
        {
            case LateralMovements.KAT:
                katMovement.gameObject.SetActive(true);
                break;
            case LateralMovements.Teleport:
                teleportMovement.gameObject.SetActive(true);
                break;
            case LateralMovements.Continuous:
                continuousLateralMovement.gameObject.SetActive(true);
                break;
        }
    }

    private void SetRotationalMovement(RotationalMovements type)
    {
        katMovement.gameObject.SetActive(false);
        snapTurn.gameObject.SetActive(false);

        switch (type)
        {
            case RotationalMovements.KAT:
                katMovement.gameObject.SetActive(true);
                break;
            case RotationalMovements.SnapTurn:
                snapTurn.gameObject.SetActive(true);
                break;
            case RotationalMovements.Continuous:
                throw new System.NotImplementedException();
        }
    }
}
