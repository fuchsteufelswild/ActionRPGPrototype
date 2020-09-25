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

    public int Health => m_Health;
    public int MaxHealth => m_MaxHealth;
    public int Level => m_Level;
    public string HeroName => m_HeroName;
    public int AttackDamage => m_AttackDamage;
    public int Experience => m_Experience;

    public int HashCode => 
        heroInfo == null ? m_HeroName.GetHashCode() : heroInfo.HashCode();

    public HeroData(HeroInfoBase info)
    {
        IsInitialzied = true;
        heroInfo = info;

        m_HeroName = heroInfo.heroName;
        m_Experience = 0;
        m_Level = 1;
        m_Health = m_MaxHealth = heroInfo.baseHealth;
        m_AttackDamage = heroInfo.attackDamage;
    }

    public HeroData() => heroInfo = null;

    public void SetHeroData(string heroName, int health, int attackDamage, bool isEnemy)
    {
        if (IsInitialzied)
        {
            Debug.LogError("Hero already has initialized data");
            return;
        }

        IsInitialzied = true;

        IsEnemy = isEnemy;
        m_HeroName = heroName;
        m_AttackDamage = attackDamage;
        m_Health = m_MaxHealth = health;
        m_Experience = 0;
        m_Level = 1;
    }

    public string ToolTip()
    {
        StringBuilder sBuilder = null;
        if (heroInfo != null)
            sBuilder = new StringBuilder(heroInfo.GetToolTip());
        else
            sBuilder = new StringBuilder(DefaultToolTip);

        sBuilder.Replace("{NAME}", heroInfo.heroName);
        sBuilder.Replace("{DAMAGE}", m_AttackDamage.ToString());
        sBuilder.Replace("{HEALTH}", m_MaxHealth.ToString());
        sBuilder.Replace("{LEVEL}", m_Level.ToString());
        sBuilder.Replace("{EXPERIENCE}", m_Experience.ToString());

        return sBuilder.ToString();
    }
}
