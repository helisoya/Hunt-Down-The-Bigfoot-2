using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;

public class MainMenuGUI : MonoBehaviour
{
    [Header("Normal Screen")]
    [SerializeField] private GameObject normalScreenRoot;


    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreenRoot;
    [SerializeField] private TMP_InputField playerNameInput;

    [Header("Servers Screen")]
    [SerializeField] private GameObject serversScreenRoot;
    [SerializeField] private Transform serverParent;
    [SerializeField] private GameObject prefabServer;
    [SerializeField] private TMP_InputField ipInput;


    Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public NetworkManager networkManager;
    public NetworkDiscovery networkDiscovery;



    public void OpenSettings()
    {
        normalScreenRoot.SetActive(false);
        settingsScreenRoot.SetActive(true);
    }

    public void CloseSettings()
    {
        normalScreenRoot.SetActive(true);
        settingsScreenRoot.SetActive(false);
    }

    public void OpenServers()
    {
        normalScreenRoot.SetActive(false);
        serversScreenRoot.SetActive(true);
        SearchServers();
    }

    public void CloseServers()
    {
        normalScreenRoot.SetActive(true);
        serversScreenRoot.SetActive(false);
        StopSearchingServers();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        string[] ipSplit = GetLocalIPv4().Split(".");
        string ipCorrect = ipSplit[0] + "." + ipSplit[1] + "." + ipSplit[2] + ".255";
        networkDiscovery.BroadcastAddress = ipCorrect;
        ipInput.SetTextWithoutNotify(networkDiscovery.BroadcastAddress);

        if (!PlayerPrefs.HasKey("playerName"))
        {
            PlayerPrefs.SetString("playerName", "Hunter");
        }
        playerNameInput.SetTextWithoutNotify(PlayerPrefs.GetString("playerName"));
    }


    // Settings

    public void AcceptSettingsChanges()
    {
        if (playerNameInput.text != "")
        {
            PlayerPrefs.SetString("playerName", playerNameInput.text);
        }
        CloseSettings();
    }


    // Servers

    public string GetLocalIPv4()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }


    public void StopSearchingServers()
    {
        discoveredServers.Clear();
        foreach (Transform t in serverParent.transform)
        {
            Destroy(t.gameObject);
        }
        networkDiscovery.StopDiscovery();
    }


    public void SearchServers()
    {
        StopSearchingServers();
        networkDiscovery.StartDiscovery();
    }

    public void StartHost()
    {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void StartClient(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        if (discoveredServers.ContainsKey(info.serverId)) return;

        discoveredServers[info.serverId] = info;
        print("Serveur : " + info.serverId + " - " + info.uri);
        GameObject button = Instantiate(prefabServer, serverParent);
        button.GetComponent<ServerButton>().Init(info, this);
        RectTransform rect = serverParent.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * discoveredServers.Keys.Count);
    }
}
