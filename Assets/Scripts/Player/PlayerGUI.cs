using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerGUI : NetworkBehaviour
{
    [Header("Normal GUI")]
    [SerializeField] private GameObject normalGUIRoot;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image playerHealth;
    [SerializeField] private Image bigfootHealth;
    [SerializeField] private Image damageImg;
    [SerializeField] private float damageAnimationSpeed;


    [Header("Camera GUI")]
    [SerializeField] private GameObject cameraGUIRoot;
    [SerializeField] private GameObject[] cams;
    private int currentCam = 0;


    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private GameObject normalPauseRoot;
    [SerializeField] private GameObject settingsPauseRoot;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private Resolution[] resolutions;
    private bool lastScreenWasNormal;


    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardRoot;
    [SerializeField] private Transform leaderboardPrefabRoot;
    [SerializeField] private GameObject leaderboardPrefab;

    [Header("Win Screen")]
    [SerializeField] private GameObject winScreenRoot;
    [SerializeField] private Image winGoodFill;
    [SerializeField] private Image winMissedFill;
    [SerializeField] private Image winFriendlyFill;


    [Header("Other")]
    public Transform respawnPoint;


    private Coroutine dmgAnimation;

    public static PlayerGUI instance;

    [HideInInspector] public PlayerHealth playerHealthScript;



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        RefreshText(8, 32);
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].SetActive(false);
        }


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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuRoot.activeInHierarchy) ClosePauseMenu();
            else OpenPauseMenu();
        }


        if (Input.GetKeyDown(KeyCode.Tab) && !cameraGUIRoot.activeInHierarchy && !pauseMenuRoot.activeInHierarchy)
        {
            OpenLeaderboard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab) && leaderboardRoot.activeInHierarchy)
        {
            CloseLeaderboard();
        }
    }


    public void OpenWinScreen()
    {
        normalGUIRoot.SetActive(false);
        pauseMenuRoot.SetActive(false);
        cameraGUIRoot.SetActive(false);
        leaderboardRoot.SetActive(false);
        winScreenRoot.SetActive(true);

        int good = Player.gun.goodShots;
        int missed = Player.gun.missedShots;
        int friendly = Player.gun.friendlyShots;
        float total = good + missed + friendly;

        winFriendlyFill.fillAmount = friendly / total;
        winGoodFill.fillAmount = good / total;
        winMissedFill.fillAmount = missed / total;
    }


    public void OpenLeaderboard()
    {
        leaderboardRoot.SetActive(true);

        foreach (string id in Player.playerList.Keys)
        {
            Instantiate(leaderboardPrefab, leaderboardPrefabRoot).GetComponent<PlayerLeaderboard>().SetPlayer(id);
        }
    }

    public void CloseLeaderboard()
    {
        foreach (Transform child in leaderboardPrefabRoot)
        {
            Destroy(child.gameObject);
        }

        leaderboardRoot.SetActive(false);
    }


    public void OpenPauseMenu()
    {
        CloseLeaderboard();
        lastScreenWasNormal = normalGUIRoot.activeInHierarchy;
        Player.localPlayerCanMove = false;
        normalGUIRoot.SetActive(false);
        pauseMenuRoot.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        if (lastScreenWasNormal)
        {
            Player.localPlayerCanMove = true;
            normalGUIRoot.SetActive(true);
        }
        pauseMenuRoot.SetActive(false);
        normalPauseRoot.SetActive(true);
        settingsPauseRoot.SetActive(false);
    }

    public void QuitGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        normalPauseRoot.SetActive(false);
        settingsPauseRoot.SetActive(true);
    }

    public void ApplyAndCloseSettings()
    {
        Resolution res = resolutions[resolutionsDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn, res.refreshRate);
        normalPauseRoot.SetActive(true);
        settingsPauseRoot.SetActive(false);
    }

    public void OpenCameraMenu()
    {
        CloseLeaderboard();
        cams[currentCam].SetActive(true);
        Player.localPlayerCanMove = false;
        normalGUIRoot.SetActive(false);
        cameraGUIRoot.SetActive(true);
    }

    public void CloseCameraMenu()
    {
        cams[currentCam].SetActive(false);
        Player.localPlayerCanMove = true;
        normalGUIRoot.SetActive(true);
        cameraGUIRoot.SetActive(false);
    }

    public void ChangeCamera(int delta)
    {
        cams[currentCam].SetActive(false);
        currentCam = (currentCam + delta + cams.Length) % cams.Length;
        cams[currentCam].SetActive(true);
    }

    public void SetHint(string hint)
    {
        hintText.text = hint;
    }

    public void RefreshText(int ammoInClip, int totalAmmo)
    {
        ammoText.text = ammoInClip + "/" + totalAmmo;
    }

    public void UpdatePlayerHealth()
    {
        UpdatePlayerHealth(playerHealthScript.GetMaxHealth(), playerHealthScript.GetCurrentHealth());
    }

    public void UpdatePlayerHealth(float maxHealth, float currentHealth)
    {
        playerHealth.fillAmount = currentHealth / maxHealth;
    }

    public void UpdateBigfootHealth(float maxHealth, float currentHealth)
    {
        bigfootHealth.fillAmount = currentHealth / maxHealth;
    }

    public void StartDamageAnimation()
    {
        if (dmgAnimation != null)
        {
            StopCoroutine(dmgAnimation);
        }
        dmgAnimation = StartCoroutine(DamageAnimation());
    }

    IEnumerator DamageAnimation()
    {
        float alpha = damageImg.color.a;
        Color col = new Color(damageImg.color.r, damageImg.color.g, damageImg.color.b, alpha);

        while (alpha < 1)
        {
            alpha = Mathf.Clamp(alpha + Time.deltaTime * damageAnimationSpeed, 0, 1);
            col.a = alpha;
            damageImg.color = col;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.2f);

        while (alpha > 0)
        {
            alpha = Mathf.Clamp(alpha - Time.deltaTime * damageAnimationSpeed, 0, 1);
            col.a = alpha;
            damageImg.color = col;
            yield return new WaitForEndOfFrame();
        }

        dmgAnimation = null;
    }
}
