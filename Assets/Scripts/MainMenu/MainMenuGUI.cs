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
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private Resolution[] resolutions;

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

        resolutions = Screen.resolutions;
        Resolution current = Screen.currentResolution;
        int currentRes = 0;
        List<string> descs = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution tmp = resolutions[i];
            if (currentRes == 0 && current.width == tmp.width && current.height == tmp.height && current.refreshRate == tmp.refreshRate)
            {
                currentRes = i;
            }
            descs.Add(tmp.width + "x" + tmp.height + " (" + tmp.refreshRate + ")");
        }
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(descs);
        resolutionsDropdown.SetValueWithoutNotify(currentRes);
        fullscreenToggle.isOn = Screen.fullScreen;
    }


    // Settings

    public void AcceptSettingsChanges()
    {
        if (playerNameInput.text != "")
        {
            PlayerPrefs.SetString("playerName", playerNameInput.text);
        }

        Resolution res = resolutions[resolutionsDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn, res.refreshRate);

        CloseSettings();
    }


    // Servers

    public void ChangeBroadcastIP()
    {
        networkDiscovery.BroadcastAddress = ipInput.text;
        SearchServers();
    }

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
