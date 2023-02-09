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

        if (enabled)
        {
            PlayerGUI.instance.StartDamageAnimation();
            PlayerGUI.instance.UpdatePlayerHealth();
        }
        RefreshHealth();
    }

    [ClientRpc]
    public void RefreshHealth()
    {
        if (enabled)
        {
            PlayerGUI.instance.StartDamageAnimation();
            PlayerGUI.instance.UpdatePlayerHealth();
        }
    }

}
