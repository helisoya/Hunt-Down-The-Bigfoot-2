using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GolfCart : InteractableObject
{
    [SerializeField] private Transform playerRoot;
    [SyncVar] private string playerDriving = "";

    [Header("Components")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider backLeft;
    [SerializeField] private WheelCollider backRight;

    [Header("Stats")]
    [SerializeField] private float accelerationForce = 500f;
    [SerializeField] private float breakForce = 300f;
    [SerializeField] private float maxTurnAngle = 15f;
    private float currentAccelerationForce;
    private float currentBreakForce;
    private float currentTurnAngle;


    public void UpdateVehicule()
    {
        currentAccelerationForce = accelerationForce * Input.GetAxis("Vertical");
        currentBreakForce = Input.GetKeyDown(KeyCode.Space) ? breakForce : 0;
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        Command_Values(currentAccelerationForce, currentBreakForce, currentTurnAngle);
    }

    void FixedUpdate()
    {
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        frontLeft.motorTorque = currentAccelerationForce;
        frontRight.motorTorque = currentAccelerationForce;

        frontLeft.brakeTorque = currentBreakForce;
        frontRight.brakeTorque = currentBreakForce;
    }

    protected override void Interaction()
    {
        if (playerDriving != "" && Player.local.GetComponent<NetworkIdentity>().netId.ToString() == playerDriving)
        {
            Player.localDrivenVehicule = null;
            SetPlayerDriving("");
            Player.local.GetComponent<PlayerMovements>().ExitVehicule();
            Command_Values(0, breakForce, 0);
        }
        else if (playerDriving == "")
        {
            Player.localDrivenVehicule = this;
            SetPlayerDriving(Player.local.GetComponent<NetworkIdentity>().netId.ToString());
            Player.local.GetComponent<PlayerMovements>().EnterVehicule(playerRoot);
        }

    }


    [Command(requiresAuthority = false)]
    public void SetPlayerDriving(string val)
    {
        playerDriving = val;
        Rpc_SetPlayerDriving(val);
    }

    [ClientRpc]
    public void Rpc_SetPlayerDriving(string val)
    {
        playerDriving = val;
    }


    [Command(requiresAuthority = false)]
    void Command_Values(float accel, float brake, float turn)
    {
        if (backLeft != null)
        {
            currentAccelerationForce = accel;
            currentBreakForce = brake;
            currentTurnAngle = turn;
        }


        RpcClient_Values(accel, brake, turn);
    }

    [ClientRpc]
    void RpcClient_Values(float accel, float brake, float turn)
    {
        currentAccelerationForce = accel;
        currentBreakForce = brake;
        currentTurnAngle = turn;
    }
}
