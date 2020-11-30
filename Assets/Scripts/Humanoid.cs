using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Humanoid : MonoBehaviour, IHumanoid
{
    [SerializeField] private string humanoidName;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Color teamColor;

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
}