﻿using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

[System.Serializable]
public class HeroData
{
    public const string DefaultToolTip = "Name: {NAME}\nDamage: {DAMAGE}\nHealth: {HEALTH}\nLevel: {LEVEL}\nExperience: {EXPERIENCE}";

    public bool IsInitialzied { get; private set; }
    public bool IsEnemy { get; private set; }

    public Func<int, int> AttributeIncreaseFunction;

    const int MaximumExperience = 5; 

    int m_AttackDamage;
    int m_Health;
    int m_MaxHealth;
    int m_Level;
    int m_Experience;
    string m_HeroName;

    float m_ColorR;
    float m_ColorG;
    float m_ColorB;

    public int Health => m_Health;
    public int MaxHealth => m_MaxHealth;
    public int Level => m_Level;
    public string HeroName => m_HeroName;
    public Color HeroColor => new Color(m_ColorR, m_ColorG, m_ColorB);
    public int AttackDamage => m_AttackDamage;
    public int Experience => m_Experience;


    public bool CreatedFromRecipe { get; private set; }
    public int HashCode =>
        m_HeroName.GetHashCode();

    public void UpdateHealth(int newValue) => m_Health = newValue;

    public void ResetHealth() => m_Health = m_MaxHealth;

    public bool IncreaseExperience()
    {
        if (++m_Experience == MaximumExperience)
            return true;

        return false;
    }

    public void LevelUp()
    {
        m_AttackDamage = AttributeIncreaseFunction(m_AttackDamage);
        m_MaxHealth = AttributeIncreaseFunction(m_MaxHealth);
        m_Health = AttributeIncreaseFunction(m_Health);
        ++m_Level;
        m_Experience = 0;
    }

    void SetDefaults(bool isEnemy)
    {
        m_Experience = 0;
        m_Level = 1;
        IsInitialzied = true;

        if(isEnemy)
        {
            m_Experience = UnityEngine.Random.Range(0, MaximumExperience);
            m_Level = UnityEngine.Random.Range(1, 10);
        }
    }

    public HeroData(HeroInfoBase info) =>
        SetHeroData(new KeyValuePair<string, Color>(info.heroName, info.heroColor), info.baseHealth, info.attackDamage, info.isEnemy, true);

    public HeroData(KeyValuePair<string, Color> heroAttributes, int health, int attackDamage, bool isEnemy) =>
        SetHeroData(heroAttributes, health, attackDamage, isEnemy, false);

    void SetHeroData(KeyValuePair<string, Color> heroAttributes, int health, int attackDamage, bool isEnemy, bool createdFromRecipe)
    {
        if (IsInitialzied)
        {
            Debug.LogError("Hero already has initialized data");
            return;
        }
        CreatedFromRecipe = createdFromRecipe;
        AttributeIncreaseFunction = (value) => (int)(value * 1.1f);

        SetDefaults(isEnemy);

        IsEnemy = isEnemy;
        m_HeroName = heroAttributes.Key;

        Color color = heroAttributes.Value;
        (m_ColorR, m_ColorG, m_ColorB) = (color.r, color.g, color.b);

        m_AttackDamage = attackDamage;
        m_Health = m_MaxHealth = health;
    }

    public string ToolTip()
    {
        StringBuilder sBuilder = new StringBuilder(DefaultToolTip);

        sBuilder.Replace("{NAME}", m_HeroName);
        sBuilder.Replace("{DAMAGE}", m_AttackDamage.ToString());
        sBuilder.Replace("{HEALTH}", m_Health.ToString());
        sBuilder.Replace("{LEVEL}", m_Level.ToString());
        sBuilder.Replace("{EXPERIENCE}", m_Experience.ToString());

        return sBuilder.ToString();
    }
}
