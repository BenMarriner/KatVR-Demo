using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    MovementManager movementManager;

    bool katMovementOverride = false;

    MovementManager.LateralMovements latMovement;
    MovementManager.RotationalMovements rotMovement;
    float snapTurnAngle;
    float contTurnSpeed;

    public GameObject player;
    public SelectionPanel txtLatMovement;
    public SelectionPanel txtRotMovement;
    public SelectionPanel txtSnapTurnAngle;
    public SelectionPanel txtContTurnSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        movementManager = player.GetComponent<MovementManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!katMovementOverride)
            katMovementOverride = txtLatMovement.Index == (int)MovementManager.LateralMovements.KAT || txtRotMovement.Index == (int)MovementManager.RotationalMovements.KAT;
        else
            katMovementOverride = txtLatMovement.Index == (int)MovementManager.LateralMovements.KAT && txtRotMovement.Index == (int)MovementManager.RotationalMovements.KAT;

        if (katMovementOverride)
        {
            txtLatMovement.Index = (int)MovementManager.LateralMovements.KAT;
            txtRotMovement.Index = (int)MovementManager.RotationalMovements.KAT;
        }
        else
        {
            if (txtLatMovement.Index == (int)MovementManager.LateralMovements.KAT)
                txtLatMovement.Index = (int)MovementManager.LateralMovements.Teleport;
            if (txtRotMovement.Index == (int)MovementManager.RotationalMovements.KAT)
                txtRotMovement.Index = (int)MovementManager.RotationalMovements.SnapTurn;
        }

        latMovement = (MovementManager.LateralMovements)txtLatMovement.Index;
        rotMovement = (MovementManager.RotationalMovements)txtRotMovement.Index;
        snapTurnAngle = txtSnapTurnAngle.GetIndexedValueAsInteger();
        contTurnSpeed = txtContTurnSpeed.GetIndexedValueAsInteger();
    }

    public void ApplySettings()
    {
        movementManager.SetMovements(latMovement, rotMovement);
        movementManager.SetSnapTurnAngle(snapTurnAngle);
    }
}
