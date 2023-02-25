using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSound : NetworkBehaviour
{
    [SerializeField] protected AudioSource generalSource;

    [Command(requiresAuthority = false)]
    public void CmdAddSound(string filename)
    {
        RpcAddSound(filename);
    }

    [ClientRpc]
    public void RpcAddSound(string filename)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + filename);
        if (clip != null)
            generalSource.PlayOneShot(clip);
    }

}
