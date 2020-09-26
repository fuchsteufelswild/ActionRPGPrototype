using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHero : HeroBase
{
    public BattleHero target;

    public bool IsAlive => m_Hero.Health > 0;

    [SerializeField] DynamicFillBar m_HealthBar;

    public void SetTarget(GameObject target)
    {
        this.target = target.GetComponent<BattleHero>();
    }

    protected override void MakeSelection()
    {
        // Notify Attack Starts
        // Attack to target
        // Play animation of some kind
        // Spawn Attack Effect on Target
        // TakeDamage on target
        // Notify Attack Ends

        TakeDamage(10);
    }

    public void TakeDamage(int damage)
    {
        int currentHealth = m_Hero.Health;
        currentHealth = Mathf.Max(currentHealth - damage, 0);        

        m_HealthBar.SetFillAmount((float)currentHealth / m_Hero.MaxHealth);

        m_Hero.UpdateHealth(currentHealth);
        
    }

}
