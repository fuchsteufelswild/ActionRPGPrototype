using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class BattleHero : HeroBase
{
    public bool IsAlive => m_Hero.Health > 0;
    public bool IsEnemy => m_Hero.IsEnemy;

    [SerializeField] Color m_DeadColor;

    [SerializeField] DynamicFillBar m_HealthBar;
    [SerializeField] float m_AttackDuration;

    float m_OffsetX;

    Vector3 m_AttackEndPosition;
    Vector3 m_AttackBeginPosition;

    Animator m_Animator;
    RectTransform m_RectTransform;

    Func<bool, BattleHero> m_TargetPicker;

    BattleHero target;

    public bool isRewardingComplete = false;

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    public void SetTarget(GameObject target)
    {
        this.target = target.GetComponent<BattleHero>();
    }

    public void ResetBattleHero(HeroData hero)
    {
        base.SetHeroData(hero);

        float amount = (float)m_Hero.Health / m_Hero.MaxHealth;

        if(amount <= 0)
        {
            m_HeroImage.color = m_DeadColor;
        }

        m_HealthBar.SetFillAmount(amount);
    }

    protected override void Start()
    {
        base.Start();

        m_AttackBeginPosition = transform.position;
        m_RectTransform = GetComponent<RectTransform>();
        m_OffsetX = m_RectTransform.sizeDelta.x * .5f * Screen.width / 1920;

        m_AttackEndPosition = transform.position + new Vector3(m_OffsetX, 0, 0);

        m_Animator = GetComponent<Animator>();
        
        m_TargetPicker = FindObjectOfType<BattleScene>().GetRandomAliveHero;
    }

    protected override void MakeSelection()
    {
        if (BattleScene.IsAttackInProgress ||
            !BattleScene.IsPlayerTurn ||
            m_Hero.IsEnemy ||
            !IsAlive) return;

        // Notify Attack Starts
        // Attack to target
        // Play animation of some kind
        // Spawn Attack Effect on Target
        // TakeDamage on target
        // Notify Attack Ends

        PerformAttack();
    }

    public void PerformAttack()
    {
        int attackDirection = m_Hero.IsEnemy ? -1 : 1;
        m_OffsetX = m_RectTransform.sizeDelta.x * .5f * Screen.width / 1920;
        m_AttackBeginPosition = transform.position;
        m_AttackEndPosition = m_AttackBeginPosition + new Vector3(attackDirection * m_OffsetX , 0, 0);

        EventMessenger.NotifyEvent(FightEvents.ATTACK_SIGNAL_GIVEN);

        target = m_TargetPicker(!m_Hero.IsEnemy);

        if (target == null)
            return;

        StartCoroutine(AttackRoutine());
    }

    void DoingDamage()
    {
        EventMessenger.NotifyEvent<BattleHero, int>(FightEvents.DAMAGE_DONE, target, m_Hero.AttackDamage);
    }

    void AttackEnded()
    {
        EventMessenger.NotifyEvent(FightEvents.ATTACK_COMPLETED);
    }

    public bool TakeDamage(int damage)
    {
        int currentHealth = m_Hero.Health;
        currentHealth = Mathf.Max(currentHealth - damage, 0);        

        m_HealthBar.SetFillAmount((float)currentHealth / m_Hero.MaxHealth);

        m_Hero.UpdateHealth(currentHealth);

        if(!IsAlive)
        {
            m_HeroImage.color = m_DeadColor;
            EventMessenger.NotifyEvent<BattleHero>(FightEvents.HERO_DIED, this);
        }

        return !IsAlive;
    }

    public void RewardHero()
    {
        isRewardingComplete = false;

        int attackIncrease = -1;
        int healthIncrease = -1;
        int levelUp = -1;
        if(m_Hero.IncreaseExperience())
        {
            levelUp = 1;
            attackIncrease = (int)(m_Hero.AttackDamage * .1f);
            healthIncrease = (int)(m_Hero.MaxHealth * .1f);

            m_Hero.LevelUp();
        }

        StartCoroutine(RewardRoutine(attackIncrease, healthIncrease, levelUp));
    }

    void SpawnAttributeChange(StringBuilder builder, int value)
    {
        builder.Replace("{INCREASE}", value.ToString());

        AttributeChange attribute = BattleScene.AttribueChangeEffectPool.GetItem();
        attribute.PrepareForActivation(transform, true, builder.ToString());
        attribute.gameObject.SetActive(true);
    }

    IEnumerator RewardRoutine(int attackIncrease, int healthIncrease, int levelUp)
    {

        StringBuilder builder = new StringBuilder(AttributeChange.EXPERIENCE_INCREASE_TEXT);
        SpawnAttributeChange(builder, 1);

        builder.Clear();
        yield return waitForSeconds;

        if (SpawnIfSuitable(levelUp, AttributeChange.LEVEL_INCREASE_TEXT))
            yield return waitForSeconds;

        if (SpawnIfSuitable(attackIncrease, AttributeChange.ATTACK_DAMAGE_INCREASE_TEXT))
            yield return waitForSeconds;

        if (SpawnIfSuitable(healthIncrease, AttributeChange.HEALTH_INCREASE_TEXT))
            yield return waitForSeconds;

        yield return waitForSeconds;

        isRewardingComplete = true;

        bool SpawnIfSuitable(int value, string text)
        {
            if(value != -1)
            {
                builder.Append(text);
                SpawnAttributeChange(builder, value);

                builder.Clear();
                return true;
            }

            return false;
        }
    }

    IEnumerator AttackRoutine()
    {
        float timePassed = 0.0f;

        Vector3 offset = m_AttackEndPosition - m_AttackBeginPosition;
        float halfDuration = m_AttackDuration * .5f;

        while(timePassed <= halfDuration)
        {
            transform.position = Vector3.Lerp(transform.position, m_AttackEndPosition, timePassed / halfDuration);

            timePassed += Time.deltaTime;

            yield return waitForEndOfFrame;
        }

        DoingDamage();

        yield return waitForEndOfFrame;

        timePassed = .0f;

        while(timePassed <= halfDuration)
        {
            transform.position = Vector3.Lerp(transform.position, m_AttackBeginPosition, timePassed / halfDuration);

            timePassed += Time.deltaTime;

            yield return waitForEndOfFrame;
        }

        Invoke("AttackEnded", 0.5f);
    }

}
