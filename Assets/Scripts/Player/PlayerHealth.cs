using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SyncVar] private int currentHealth;

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }


    [Command(requiresAuthority = false)]
    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);

        bool isDead = false;
        if (currentHealth == 0)
        {
            isDead = true;
            currentHealth = maxHealth;
        }

        if (enabled)
        {
            PlayerGUI.instance.StartDamageAnimation();
            PlayerGUI.instance.UpdatePlayerHealth();
        }

        if (isLocalPlayer && isDead)
        {
            transform.position = PlayerGUI.instance.respawnPoint.position;
        }

        RefreshHealth(isDead);
    }

    [ClientRpc]
    public void RefreshHealth(bool isDead)
    {
        if (enabled)
        {
            PlayerGUI.instance.StartDamageAnimation();
            PlayerGUI.instance.UpdatePlayerHealth();
        }

        if (isLocalPlayer && isDead)
        {
            transform.position = PlayerGUI.instance.respawnPoint.position;
        }
    }

}
