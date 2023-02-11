using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InteractableObject : NetworkBehaviour
{
    protected bool playerInZone;

    [SerializeField] protected string hint;


    // Update is called once per frame
    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            Interaction();
        }
    }

    protected virtual void Interaction()
    {
        playerInZone = false;
        PlayerGUI.instance.SetHint("");
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && col.gameObject == Player.local)
        {
            playerInZone = true;
            PlayerGUI.instance.SetHint(hint);
        }
    }

    protected virtual void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && col.gameObject == Player.local)
        {
            playerInZone = false;
            PlayerGUI.instance.SetHint("");
        }
    }
}
