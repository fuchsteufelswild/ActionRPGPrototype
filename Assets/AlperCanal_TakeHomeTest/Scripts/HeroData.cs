using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class HeroData
{
    HeroInfoBase heroInfo;

    public const string DefaultToolTip = "Name: {NAME}\nDamage: {DAMAGE}\nHealth: {HEALTH}\nLevel: {LEVEL}\nExperience: {EXPERIENCE}";

    public bool IsInitialzied { get; private set; }
    public bool IsEnemy { get; private set; }

    int m_AttackDamage;
    int m_Health;
    int m_MaxHealth;
    int m_Level;
    int m_Experience;
    string m_HeroName;
    Color m_Color;

    public int Health { get => m_Health; private set => m_Health = value; } 
    public int MaxHealth => m_MaxHealth;
    public int Level => m_Level;
    public string HeroName => m_HeroName;
    public Color HeroColor => m_Color;
    public int AttackDamage => m_AttackDamage;
    public int Experience => m_Experience;

    public int HashCode => 
        heroInfo == null ? m_HeroName.GetHashCode() : heroInfo.HashCode();

    public void UpdateHealth(int newValue) => Health = newValue;

    void SetDefaults()
    {
        m_Experience = 0;
        m_Level = 1;
        IsInitialzied = true;
    }

    public HeroData(HeroInfoBase info)
    {
        heroInfo = info;

        SetHeroData(new KeyValuePair<string, Color>(heroInfo.heroName, heroInfo.heroColor), heroInfo.baseHealth, heroInfo.attackDamage, heroInfo.isEnemy);
    }

    public HeroData() => heroInfo = null;

    public void SetHeroData(KeyValuePair<string, Color> heroAttributes, int health, int attackDamage, bool isEnemy)
    {
        if (IsInitialzied)
        {
            Debug.LogError("Hero already has initialized data");
            return;
        }

        SetDefaults();

        IsEnemy = isEnemy;
        m_HeroName = heroAttributes.Key;
        m_Color = heroAttributes.Value;
        m_AttackDamage = attackDamage;
        m_Health = m_MaxHealth = health;
    }

    public string ToolTip()
    {
        StringBuilder sBuilder = null;
        if (heroInfo != null)
            sBuilder = new StringBuilder(heroInfo.GetToolTip());
        else
            sBuilder = new StringBuilder(DefaultToolTip);

        sBuilder.Replace("{NAME}", m_HeroName);
        sBuilder.Replace("{DAMAGE}", m_AttackDamage.ToString());
        sBuilder.Replace("{HEALTH}", m_MaxHealth.ToString());
        sBuilder.Replace("{LEVEL}", m_Level.ToString());
        sBuilder.Replace("{EXPERIENCE}", m_Experience.ToString());

        return sBuilder.ToString();
    }
}
