using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{

    [SerializeField] private Component[] listScriptsToDisableIfNotLocal;
    [SerializeField] private GameObject[] listObjectsToDisableIfLocal;
    [SerializeField] private GameObject[] listObjectsToDisableIfNotLocal;

    [SerializeField] private PlayerNameText playerName;


    public override void OnStopClient()
    {
        base.OnStopClient();

        Player.RemovePlayer(GetComponent<NetworkIdentity>().netId.ToString());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Player.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(), gameObject);

        gameObject.name = "Hunter (" + GetComponent<NetworkIdentity>().netId.ToString() + ")";

        if (!isLocalPlayer)
        {
            foreach (MonoBehaviour behaviour in listScriptsToDisableIfNotLocal)
            {
                behaviour.enabled = false;
            }

            foreach (GameObject obj in listObjectsToDisableIfNotLocal)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            Player.localPlayerCanMove = true;
            PlayerGUI.instance.playerHealthScript = GetComponent<PlayerHealth>();
            Player.local = gameObject;
            foreach (GameObject obj in listObjectsToDisableIfLocal)
            {
                obj.SetActive(false);
            }

            Command_ChangePlayerName(PlayerPrefs.GetString("playerName"));
        }
    }

    [Command(requiresAuthority = false)]
    void Command_ChangePlayerName(string name)
    {
        playerName.SetText(name);
        RpcClient_ChangePlayerName(name);
    }

    [ClientRpc]
    void RpcClient_ChangePlayerName(string name)
    {
        playerName.SetText(name);
    }
}
