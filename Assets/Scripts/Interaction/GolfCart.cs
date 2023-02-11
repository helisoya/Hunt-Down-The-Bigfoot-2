using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GolfCart : InteractableObject
{
    [SerializeField] private Transform playerRoot;
    [SyncVar] private string playerDriving = "";
    protected override void Interaction()
    {
        if (playerDriving != "" && Player.local.GetComponent<NetworkIdentity>().netId.ToString() == playerDriving)
        {
            SetPlayerDriving("");
            Player.local.GetComponent<PlayerMovements>().ExitVehicule();
        }
        else if (playerDriving == "")
        {
            SetPlayerDriving(Player.local.GetComponent<NetworkIdentity>().netId.ToString());
            Player.local.GetComponent<PlayerMovements>().EnterVehicule(playerRoot);
        }

    }


    [Command(requiresAuthority = false)]
    public void SetPlayerDriving(string val)
    {
        playerDriving = val;
    }
}
