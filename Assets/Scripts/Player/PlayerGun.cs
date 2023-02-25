using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerGun : NetworkBehaviour
{
    [Header("Clips")]
    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxAmmoInClip;
    private int currentAmmoInClip;
    private int currentTotalAmmo;

    [Header("Fire")]
    [SerializeField] private int dmg = 1;
    [SerializeField] private float maxDistance = 100;
    [SerializeField] private LayerMask mask;

    [Header("Animations Timing")]
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireTime;
    private float lastAction = 0;
    private float waitFor = 0;

    [Header("Components")]
    [SerializeField] private NetworkAnimator bodyAnimator;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private ParticleSystem flare;
    [SerializeField] private PlayerSound sound;


    [SyncVar, HideInInspector] public int goodShots;
    [SyncVar, HideInInspector] public int missedShots;
    [SyncVar, HideInInspector] public int friendlyShots;


    public bool canRefill
    {
        get
        {
            return currentTotalAmmo < maxAmmo;
        }
    }

    void Start()
    {
        currentAmmoInClip = maxAmmoInClip;
        currentTotalAmmo = maxAmmoInClip * 4;
        Player.gun = this;
    }

    void Update()
    {
        if (!Player.localPlayerCanMove || Player.localPlayerInVehicule) return;

        if (Time.time - lastAction < waitFor || Input.GetKey(KeyCode.LeftShift)) return;

        if (currentAmmoInClip > 0 && Input.GetMouseButtonDown(0))
        {
            // Tir
            Fire();
        }
        else if (currentAmmoInClip < maxAmmoInClip && Input.GetKeyDown(KeyCode.R))
        {
            // Recharger
            Reload();
        }
    }

    void Reload()
    {
        waitFor = reloadTime;
        lastAction = Time.time;
        handAnimator.SetTrigger("reload");
        bodyAnimator.SetTrigger("reload");
        sound.CmdAddSound("reload");
        while (currentAmmoInClip < maxAmmoInClip && currentTotalAmmo > 0)
        {
            currentAmmoInClip++;
            currentTotalAmmo--;
        }
        RefreshText();
    }

    public void RefillAmmo()
    {
        currentTotalAmmo = maxAmmo;
        RefreshText();
        sound.CmdAddSound("ammoPickup");
    }

    void Fire()
    {
        waitFor = fireTime;
        lastAction = Time.time;
        handAnimator.SetTrigger("fire");
        bodyAnimator.SetTrigger("fire");
        currentAmmoInClip--;
        flare.Play();
        RefreshText();

        sound.CmdAddSound("Gunshot");

        Transform cam = Camera.main.transform;


        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, mask))
        {
            if (hit.transform.tag == "Bigfoot")
            {
                // Bigfoot hurt
                Command_AddPoints(1, 0, 0);
                BigfootAI.instance.TakeDamage(dmg);
            }
            else if (hit.transform.tag == "Player")
            {
                // Hit Player
                Command_AddPoints(0, 0, 1);
                hit.transform.GetComponent<PlayerHealth>().TakeDamage(dmg);
            }
        }
        else
        {
            Command_AddPoints(0, 1, 0);
        }
    }


    [Command(requiresAuthority = false)]
    void Command_AddPoints(int good, int missed, int friendly)
    {
        goodShots += good;
        missedShots += missed;
        friendlyShots += friendly;
    }


    void RefreshText()
    {
        PlayerGUI.instance.RefreshText(currentAmmoInClip, currentTotalAmmo);
    }

}
