using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
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
    private bool lastScreenWasNormal;

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
            cams[i].SetActive(i == 0);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuRoot.activeInHierarchy) ClosePauseMenu();
            else OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
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
    }

    public void QuitGame()
    {

    }

    public void OpenSettings()
    {

    }

    public void OpenCameraMenu()
    {
        Player.localPlayerCanMove = false;
        normalGUIRoot.SetActive(false);
        cameraGUIRoot.SetActive(true);
    }

    public void CloseCameraMenu()
    {
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
