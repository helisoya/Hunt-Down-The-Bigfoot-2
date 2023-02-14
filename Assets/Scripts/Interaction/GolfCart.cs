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
    [SyncVar] private float currentAccelerationForce;
    [SyncVar] float currentBreakForce;
    [SyncVar] float currentTurnAngle;


    public void UpdateVehicule()
    {
        currentAccelerationForce = accelerationForce * Input.GetAxis("Vertical");
        currentBreakForce = Input.GetKey(KeyCode.Space) ? breakForce : 0;
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        //print(currentAccelerationForce + " " + currentBreakForce + " " + currentTurnAngle);

        Command_SetValues(currentAccelerationForce, currentBreakForce, currentTurnAngle);
    }

    [Command(requiresAuthority = false)]
    public void CheckZRotation()
    {
        if (transform.eulerAngles.z != 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }

    [Command(requiresAuthority = false)]
    void Command_SetValues(float accel, float brake, float turn)
    {
        currentAccelerationForce = accel;
        currentBreakForce = brake;
        currentTurnAngle = turn;

        frontLeft.steerAngle = turn;
        frontRight.steerAngle = turn;

        frontLeft.motorTorque = accel;
        frontRight.motorTorque = accel;

        frontLeft.brakeTorque = brake;
        frontRight.brakeTorque = brake;
    }



    protected override void Interaction()
    {
        if (playerDriving != "" && Player.local.GetComponent<NetworkIdentity>().netId.ToString() == playerDriving)
        {
            Player.localDrivenVehicule = null;
            SetPlayerDriving("");
            Player.local.GetComponent<PlayerMovements>().ExitVehicule();
            Command_SetValues(0, breakForce, 0);
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

}
