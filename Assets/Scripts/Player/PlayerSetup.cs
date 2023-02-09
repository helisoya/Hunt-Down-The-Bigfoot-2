using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{

    [SerializeField] private Component[] listScriptsToDisableIfNotLocal;
    [SerializeField] private GameObject[] listObjectsToDisableIfLocal;
    [SerializeField] private GameObject[] listObjectsToDisableIfNotLocal;



    public override void OnStopClient()
    {
        base.OnStopClient();

        Player.RemovePlayer(GetComponent<NetworkIdentity>().netId.ToString());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Player.AddPlayer(GetComponent<NetworkIdentity>().netId.ToString(), gameObject);

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
        }
    }
}
