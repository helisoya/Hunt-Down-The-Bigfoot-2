using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameText : MonoBehaviour
{
    void Update()
    {
        if (Player.local == null) return;
        transform.LookAt(Player.local.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }
}
