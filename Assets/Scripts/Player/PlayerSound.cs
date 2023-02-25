using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSound : NetworkSound
{

    [SerializeField] private AudioSource walkSource;

    public bool isMoveSoundPlaying
    {
        get { return walkSource.enabled; }
    }




    [Command(requiresAuthority = false)]
    public void CmdSetWalkSound(bool value)
    {
        RpcSetWalkSound(value);
    }

    [ClientRpc]
    public void RpcSetWalkSound(bool value)
    {
        walkSource.enabled = value;
    }
}
