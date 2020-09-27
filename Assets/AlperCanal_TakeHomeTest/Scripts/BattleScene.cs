using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;

public class BattleScene : MonoBehaviour
{
    const string PLAYER_TURN_TEXT = "PLAYER TURN";
    const string ENEMY_TURN_TEXT = "ENEMY TURN";
    const string VICTORY_TEXT = "VICTORY!";
    const string DEFEAT_TEXT = "DEFEAT";

    [SerializeField] BattleHero[] m_PlayerHeroes;
    [SerializeField] BattleHero[] m_EnemyHeroes;

    [SerializeField] GameObject m_AttackEffectPrefab;
    [SerializeField] GameObject m_AttribueChangeEffectPrefab;

    [SerializeField] Transform m_AttackEffects;
    [SerializeField] Transform m_AttributeChangeEffects;

    [SerializeField] Button m_BackButton;
    [SerializeField] Text m_TurnText;

    public static bool IsAttackInProgress = false;
    public static bool IsPlayerTurn = true;
    public bool IsFighting => m_AlivePlayerHeroCount != 0 && m_AliveEnemyHeroCount != 0;

    GameObjectPool<AttackEffect> m_AttackEffecPool;
    public static GameObjectPool<AttributeChange> AttribueChangeEffectPool;

    public MissionManager.FightSettings CurrentFightSettings { get; set; }
    

    int m_InitialPoolSize = 3;
    int m_AlivePlayerHeroCount = 0;
    int m_AliveEnemyHeroCount = 0;

    void OnFightStart()
    {
        // m_BackButton.gameObject.SetActive(false);
        m_TurnText.text = PLAYER_TURN_TEXT;
    }

    public void UpdateHeroes(HeroData[] playerHeroes, HeroData[] enemyHeroes)
    {
        IsAttackInProgress = false;
        IsPlayerTurn = true;

        for (int i = 0; i < playerHeroes.Length; ++i)
            m_PlayerHeroes[i].ResetBattleHero(playerHeroes[i]);

        for (int i = 0; i < enemyHeroes.Length; ++i)
            m_EnemyHeroes[i].ResetBattleHero(enemyHeroes[i]);

        m_AlivePlayerHeroCount = playerHeroes.Length;
        m_AliveEnemyHeroCount = enemyHeroes.Length;

        OnFightStart();
    }

    public void PrepareBattleScene(MissionManager.FightSettings fightSettings)
    {
        UpdateHeroes(fightSettings.playerSide, fightSettings.enemySide);

        CurrentFightSettings = fightSettings;
    }

    BattleHero GetHeroWithCondition(BattleHero[] battleHeroes, Func<BattleHero, bool> predicate)
    {
        BattleHero[] aliveHeroes = battleHeroes.Where(hero => predicate(hero))
                                               .ToArray();

        if (aliveHeroes.Length > 0)
            return aliveHeroes[UnityEngine.Random.Range(0, aliveHeroes.Length)];

        return null;
    }

    BattleHero GetAliveHero(BattleHero[] battleHeroes) =>
        GetHeroWithCondition(battleHeroes, hero => hero.IsAlive);
    
    public BattleHero GetRandomAliveHero(bool isEnemy)
    {
        if (isEnemy) return GetAliveHero(m_EnemyHeroes);
        return GetAliveHero(m_PlayerHeroes);
    }

    private void Start()
    {
        Debug.Assert(m_PlayerHeroes.Length >= MissionManager.PLAYER_SIDE_COUNT &&
                     m_EnemyHeroes.Length >= MissionManager.ENEMY_SIDE_COUNT);

        m_AttackEffecPool = new GameObjectPool<AttackEffect>(3, m_AttackEffects, m_AttackEffectPrefab);
        AttribueChangeEffectPool = new GameObjectPool<AttributeChange>(4, m_AttributeChangeEffects, m_AttribueChangeEffectPrefab);

        m_AttackEffecPool.DeactivateAll();
        AttribueChangeEffectPool.DeactivateAll();

        EventMessenger.AddListener(FightEvents.ATTACK_SIGNAL_GIVEN, AttackSignalGiven);
        EventMessenger.AddListener(FightEvents.ATTACK_COMPLETED, AttackCompleted);
        EventMessenger.AddListener<BattleHero, int>(FightEvents.DAMAGE_DONE, DamageDoneOnHero);
        EventMessenger.AddListener<BattleHero>(FightEvents.HERO_DIED, OnHeroDied);
        EventMessenger.AddListener(FightEvents.ALL_ALLY_DEAD, OnDefeat);
        EventMessenger.AddListener(FightEvents.ALL_ENEMY_DEAD, OnVictory);
    }

    IEnumerator VictoryRoutine()
    {
        m_TurnText.text = VICTORY_TEXT;

        BattleHero[] alivePlayerHeroes = m_PlayerHeroes.Where(hero => hero.IsAlive)
                                                       .ToArray();

        if (alivePlayerHeroes != null)
        {

            foreach (BattleHero hero in alivePlayerHeroes)
                hero.RewardHero();

            CurrentFightSettings.isRewardingComplete = true;

            while (true)
            {
                bool allReady = true;

                foreach (BattleHero hero in alivePlayerHeroes)
                {
                    if (!hero.isRewardingComplete)
                    {
                        allReady = false;
                        break;
                    }
                }

                if (allReady)
                    break;

                yield return null;
            }
        }
        
        EventMessenger.NotifyEvent(SceneEvents.REWARDING_COMPLETE);

    }

    IEnumerator DefatRoutine()
    {
        m_TurnText.text = DEFEAT_TEXT;

        CurrentFightSettings.isRewardingComplete = true;

        yield return new WaitForSeconds(2f);

        EventMessenger.NotifyEvent(SceneEvents.REWARDING_COMPLETE);
    }

    void OnVictory()
    {
        StartCoroutine(VictoryRoutine());
        
    }
    

    void OnDefeat()
    {
        StartCoroutine(DefatRoutine());
    }

    void EnemyPlay()
    {
        if (m_PlayerHeroes.Length <= 0) return;

        BattleHero enemy = GetAliveHero(m_EnemyHeroes);

        enemy?.PerformAttack();
    }

    void UpdateTurnText()
    {
        if (IsPlayerTurn)
            m_TurnText.text = PLAYER_TURN_TEXT;
        else
            m_TurnText.text = ENEMY_TURN_TEXT;
    }

    void AttackCompleted()
    {
        IsAttackInProgress = false;

        CurrentFightSettings.isPlayerTurn = !CurrentFightSettings.isPlayerTurn;
        IsPlayerTurn = CurrentFightSettings.isPlayerTurn;

        if (!IsPlayerTurn)
            EnemyPlay();

        if(m_AliveEnemyHeroCount != 0 &&
           m_AlivePlayerHeroCount != 0)
            UpdateTurnText();

        // Save Game State
    }

    void AttackSignalGiven()
    {
        IsAttackInProgress = true;
    }

    void OnHeroDied(BattleHero hero)
    {
        if (hero.IsEnemy)
        {
            if (--m_AliveEnemyHeroCount <= 0)
                EventMessenger.NotifyEvent(FightEvents.ALL_ENEMY_DEAD);
        }
        else if (--m_AlivePlayerHeroCount <= 0)
            EventMessenger.NotifyEvent(FightEvents.ALL_ALLY_DEAD);
    }

    void DamageDoneOnHero(BattleHero hero, int damage)
    {
        AttackEffect attackEffect = m_AttackEffecPool.GetItem();
        attackEffect.transform.position = hero.transform.position;
        attackEffect.RegulateSize(hero.GetComponent<RectTransform>());
        attackEffect.gameObject.SetActive(true);

        StringBuilder builder = new StringBuilder(AttributeChange.HEALTH_DECREASE_TEXT);
        builder.Replace("{DECREASE}", damage.ToString());

        AttributeChange attributeChange = AttribueChangeEffectPool.GetItem();

        attributeChange.PrepareForActivation(hero.transform, false, builder.ToString());
        attributeChange.gameObject.SetActive(true);


        bool isDead = hero.TakeDamage(damage);
    }
}
