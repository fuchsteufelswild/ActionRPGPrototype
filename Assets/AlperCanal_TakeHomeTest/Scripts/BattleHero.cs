using System.Collections;
using UnityEngine;
using System;
using System.Text;

public class BattleHero : HeroBase
{
    const float Multiplier = 0.00026f; // 1 / (1920 * 2)

    [SerializeField] Color m_DeadColor;

    [SerializeField] DynamicFillBar m_HealthBar;
    [SerializeField] float m_AttackDuration;

    public bool IsAlive => m_Hero.Health > 0;
    public bool IsEnemy => m_Hero.IsEnemy;
    public bool IsRewardingComplete { get; private set; }

    public int selectedTargetIndex;

    float OffsetX
    {
        get
        {
            int attackDirection = m_Hero != null ? (m_Hero.IsEnemy ? -1 : 1) : 1;
            return attackDirection * m_RectTransform.sizeDelta.x * Multiplier * Screen.width;
        }
    }

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    Vector3 m_AttackEndPosition;
    Vector3 m_AttackBeginPosition;

    Animator m_Animator;
    RectTransform m_RectTransform;

    Func<bool, BattleHero> m_TargetPicker;

    BattleHero target;

    public void ResetBattleHero(HeroData hero)
    {
        base.SetHeroData(hero);

        float amount = (float)m_Hero.Health / m_Hero.MaxHealth;
        selectedTargetIndex = -1;
        if (amount <= 0)
        {
            m_HeroImage.color = m_DeadColor;
        }

        m_HealthBar.SetFillAmount(amount);
    }

    void SetEndPosition() =>
        m_AttackEndPosition = transform.position + new Vector3(OffsetX, 0, 0);

    protected override void Start()
    {
        base.Start();

        m_AttackBeginPosition = transform.position;
        m_RectTransform = GetComponent<RectTransform>();
        SetEndPosition();

        m_Animator = GetComponent<Animator>();        
        m_TargetPicker = FindObjectOfType<BattleScene>().GetRandomAliveHero;
    }

    protected override void MakeSelection()
    {
        if (BattleScene.IsAttackInProgress ||
            !BattleScene.IsPlayerTurn ||
            m_Hero.IsEnemy ||
            !IsAlive) return;

        PerformAttack();
    }

    public void PerformAttack()
    {
        m_AttackBeginPosition = transform.position;
        SetEndPosition();

        EventMessenger.NotifyEvent(FightEvents.ATTACK_SIGNAL_GIVEN);

        if (selectedTargetIndex == -1)
            target = m_TargetPicker(!m_Hero.IsEnemy);
        else
            target = FindObjectOfType<BattleScene>().GetBattleHeroOnIndex(selectedTargetIndex); // This will be called only once on first attack upon loading

        if (target == null) return;

        StartCoroutine(AttackRoutine());
    }

    void DoingDamage() =>
        EventMessenger.NotifyEvent<BattleHero, int>(FightEvents.DAMAGE_DONE, target, m_Hero.AttackDamage);

    void AttackEnded()
    {
        target = null;
        selectedTargetIndex = -1;
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
        IsRewardingComplete = false;

        int attackIncrease = -1;
        int healthIncrease = -1;
        int levelUp = -1;
        if(m_Hero.IncreaseExperience())
        {
            levelUp = 1;
            attackIncrease = m_Hero.AttributeIncreaseFunction(m_Hero.AttackDamage) - m_Hero.AttackDamage;
            healthIncrease = m_Hero.AttributeIncreaseFunction(m_Hero.MaxHealth) - m_Hero.MaxHealth;

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
        StringBuilder builder = new StringBuilder("");

        SpawnIfSuitable(1, AttributeChange.EXPERIENCE_INCREASE_TEXT);
        yield return waitForSeconds;

        if (SpawnIfSuitable(levelUp, AttributeChange.LEVEL_INCREASE_TEXT))
            yield return waitForSeconds;

        if (SpawnIfSuitable(attackIncrease, AttributeChange.ATTACK_DAMAGE_INCREASE_TEXT))
            yield return waitForSeconds;

        if (SpawnIfSuitable(healthIncrease, AttributeChange.HEALTH_INCREASE_TEXT))
            yield return waitForSeconds;

        IsRewardingComplete = true;

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
