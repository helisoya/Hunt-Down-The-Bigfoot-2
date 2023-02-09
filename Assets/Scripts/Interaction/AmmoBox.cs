using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : InteractableObject
{
    protected override void Interaction()
    {
        base.Interaction();
        Player.gun.RefillAmmo();
    }


    protected override void OnTriggerEnter(Collider col)
    {
        if (Player.gun.canRefill && col.tag == "Player")
        {
            playerInZone = true;
            PlayerGUI.instance.SetHint(hint);
        }
    }
}
