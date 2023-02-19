using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLeaderboard : MonoBehaviour
{
    [SerializeField] private Image friendlyImg;
    [SerializeField] private Image goodImg;
    [SerializeField] private TextMeshProUGUI playerName;

    public void SetPlayer(string id)
    {
        GameObject obj = Player.playerList[id];
        playerName.text = obj.GetComponent<PlayerSetup>().username;

        PlayerGun stats = obj.GetComponent<PlayerGun>();
        int missed = stats.missedShots;
        int good = stats.goodShots;
        int friendly = stats.friendlyShots;
        float total = missed + good + friendly;

        friendlyImg.fillAmount = (float)(good + friendly) / total;
        goodImg.fillAmount = (float)(good) / total;
    }
}
