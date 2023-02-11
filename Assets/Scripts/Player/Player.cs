using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public static bool localPlayerCanMove;
    public static bool localPlayerInVehicule;

    public static GameObject local;
    public static PlayerGun gun;

    public static Dictionary<string, GameObject> playerList;

    public static void AddPlayer(string name, GameObject obj)
    {
        if (playerList == null) playerList = new Dictionary<string, GameObject>();
        playerList.Add(name, obj);
    }


    public static void RemovePlayer(string name)
    {
        if (playerList == null) playerList = new Dictionary<string, GameObject>();
        playerList.Remove(name);
    }


    public static GameObject GetRandomPlayer()
    {
        List<string> keyList = new List<string>(playerList.Keys);
        return playerList[keyList[Random.Range(0, keyList.Count)]];
    }
}
