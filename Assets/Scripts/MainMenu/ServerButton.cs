using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using TMPro;

public class ServerButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private ServerResponse response;
    private MainMenuGUI parent;

    public void Init(ServerResponse info, MainMenuGUI list)
    {
        text.text = info.EndPoint.Address.ToString();
        response = info;
        parent = list;
    }

    public void OnClick()
    {
        parent.StartClient(response);
    }
}
