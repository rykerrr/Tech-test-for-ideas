using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Humanoid : MonoBehaviour, IHumanoid
{
    [SerializeField] private Transform healthBar;
    [SerializeField] private string humanoidName;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Color teamColor;

    private Vector3 healthBarOffset;
    
    public string HumanoidName
    {
        get => humanoidName;
        private set { humanoidName = value; }
    }

    public int Health
    {
        get => health;
        private set { health = value; }
    }

    public int MaxHealth
    {
        get => maxHealth;
        private set { maxHealth = value; }
    }

    public Color TeamColor
    {
        get => teamColor;
        private set { teamColor = value; }
    }

    private MeshRenderer healthBarMr;
    private float healthBarSizeMult;

    private void Awake()
    {
        // Debug.LogWarning(healthBar);

        if (healthBar)
        {
            healthBarSizeMult = healthBar.localScale.z;
            healthBarMr = healthBar.GetComponentInChildren<MeshRenderer>();
            healthBarOffset = healthBar.localPosition;
            
            SetHealthbarHealth(health, maxHealth);
        }
    }

    private void SetHealthbarHealth(int curHealth, int maxHealth)
    {
        float healthRatio = (float) curHealth / (float) maxHealth;
        // Debug.Log(healthRatio);

        healthBar.localPosition = healthBarOffset;
        healthBar.localScale =
            new Vector3(healthBar.localScale.x, healthBar.localScale.y, healthRatio * healthBarSizeMult);

        if (healthRatio < 0.05f)
        {
            healthBarMr.material.color = Color.black;
        }
        else if (healthRatio < 0.35f)
        {
            healthBarMr.material.color = Color.red;
        }
        else if (healthRatio < 0.65f)
        {
            healthBarMr.material.color = Color.yellow;
        }
        else
        {
            healthBarMr.material.color = Color.green;
        }
    }

    public void TakeDamage(IDamager damager) // different effects for type of damage later on
    {
        if (health - damager.Damage <= 0)
        {
            // Debug.LogWarning(humanoidName + ", " + name + " has died.");
            gameObject.SetActive(false);
            // Debug.Break();
        }
        else
        {
            health -= damager.Damage;
        }

        SetHealthbarHealth(health, maxHealth);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        SetHealthbarHealth(health, maxHealth);
    }
}
#pragma warning restore 0649