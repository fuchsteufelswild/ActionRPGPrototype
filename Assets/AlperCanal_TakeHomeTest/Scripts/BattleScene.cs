﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Text;
using TMPro;

public class BattleScene : MonoBehaviour
{
    const string PLAYER_TURN_TEXT = "PLAYER TURN";
    const string ENEMY_TURN_TEXT = "ENEMY TURN";
    const string VICTORY_TEXT = "VICTORY!";
    const string DEFEAT_TEXT = "DEFEAT";

    public static bool IsAttackInProgress = false;
    public static bool IsPlayerTurn = true;

    [SerializeField] BattleHero[] m_PlayerHeroes;
    [SerializeField] BattleHero[] m_EnemyHeroes;

    [SerializeField] GameObject m_AttackEffectPrefab;
    [SerializeField] GameObject m_AttribueChangeEffectPrefab;

    [SerializeField] Transform m_AttackEffects;
    [SerializeField] Transform m_AttributeChangeEffects;

    [SerializeField] GameObject m_BackButton;
    [SerializeField] TextMeshProUGUI m_TurnText;

    
    public bool IsFighting => m_AlivePlayerHeroCount != 0 && m_AliveEnemyHeroCount != 0;

    GameObjectPool<AttackEffect> m_AttackEffecPool;
    public static GameObjectPool<AttributeChange> AttribueChangeEffectPool;

    MissionManager.FightSettings CurrentFightSettings { get; set; }

    int m_InitialPoolSize = 3;
    int m_AlivePlayerHeroCount = 0;
    int m_AliveEnemyHeroCount = 0;

    int FindHeroIndex(BattleHero[] heroes, BattleHero hero)
    {
        for (int i = 0; i < heroes.Length; ++i)
            if (heroes[i] == hero)
                return i;

        return -1;
    }

    BattleHero FindBattleHeroOnIndex(BattleHero[] heroes, int index)
    {
        if(index >= 0 &&
           index < heroes.Length)
            return heroes[index];

        return null;
    }

    public int GetIndexOfBattleHero(BattleHero hero)
    {
        int result = FindHeroIndex(m_PlayerHeroes, hero);

        if (result != -1) return result;

        return FindHeroIndex(m_EnemyHeroes, hero);
    }

    public BattleHero GetBattleHeroOnIndex(int index)
    {
        BattleHero result = FindBattleHeroOnIndex(m_PlayerHeroes, index);

        if (result != null) return result;

        return FindBattleHeroOnIndex(m_EnemyHeroes, index);
    }

    void OnFightStart()
    {
        m_BackButton.gameObject.SetActive(false);
        m_TurnText.text = PLAYER_TURN_TEXT;
    }

    void LoadBattleSceneSettings()
    {
        if (CurrentFightSettings == null) return;

        UpdateTurnText();

        if (!IsFighting)
        {
            if (!CurrentFightSettings.isRewardingComplete)
            {
                if (m_AliveEnemyHeroCount == 0) OnVictory();
                else if (m_AlivePlayerHeroCount == 0) OnDefeat();
            }
            else if(CurrentFightSettings.isFighting)
                m_BackButton.gameObject.SetActive(true);
        }   

        // Preserve latest selected enemy and its target selection
        if(CurrentFightSettings.selectedEnemyIndex != -1 &&
           CurrentFightSettings.selectedAllyHeroIndex != -1)
        {
                FindBattleHeroOnIndex(m_EnemyHeroes, CurrentFightSettings.selectedEnemyIndex).
                    selectedTargetIndex = CurrentFightSettings.selectedAllyHeroIndex;
        }

        if (!IsPlayerTurn)
            EnemyPlay();
    }

    void UpdateHeroes(HeroData[] playerHeroes, HeroData[] enemyHeroes)
    {
        m_AlivePlayerHeroCount = 0;
        m_AliveEnemyHeroCount = 0;
        for (int i = 0; i < playerHeroes.Length; ++i)
        {
            m_PlayerHeroes[i].ResetBattleHero(playerHeroes[i]);

            if (m_PlayerHeroes[i].IsAlive)
                ++m_AlivePlayerHeroCount;
        }

        for (int i = 0; i < enemyHeroes.Length; ++i)
        {
            m_EnemyHeroes[i].ResetBattleHero(enemyHeroes[i]);

            if (m_EnemyHeroes[i].IsAlive)
                ++m_AliveEnemyHeroCount;
        }

        OnFightStart();
    }

    public void PrepareBattleScene(MissionManager.FightSettings fightSettings)
    {
        UpdateHeroes(fightSettings.playerSide, fightSettings.enemySide);

        CurrentFightSettings = fightSettings;

        IsAttackInProgress = false;
        IsPlayerTurn = fightSettings.isPlayerTurn;
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

        m_BackButton = transform.Find("MainMenuButton").gameObject;
        m_BackButton.GetComponent<Button>().onClick.AddListener(() => EventMessenger.NotifyEvent(SceneEvents.REWARDING_COMPLETE));

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

        EventMessenger.AddListener(SaveEvents.LOADING_SAVE_COMPLETED, LoadBattleSceneSettings);
    }

    void EndBattle() =>
        m_BackButton.SetActive(true);

    private void OnDestroy()
    {
        EventMessenger.RemoveListener(FightEvents.ATTACK_SIGNAL_GIVEN, AttackSignalGiven);
        EventMessenger.RemoveListener(FightEvents.ATTACK_COMPLETED, AttackCompleted);
        EventMessenger.RemoveListener<BattleHero, int>(FightEvents.DAMAGE_DONE, DamageDoneOnHero);
        EventMessenger.RemoveListener<BattleHero>(FightEvents.HERO_DIED, OnHeroDied);
        EventMessenger.RemoveListener(FightEvents.ALL_ALLY_DEAD, OnDefeat);
        EventMessenger.RemoveListener(FightEvents.ALL_ENEMY_DEAD, OnVictory);
        EventMessenger.RemoveListener(SaveEvents.LOADING_SAVE_COMPLETED, LoadBattleSceneSettings);
    }

    IEnumerator VictoryRoutine()
    {
        m_TurnText.text = VICTORY_TEXT;

        BattleHero[] alivePlayerHeroes = m_PlayerHeroes.Where(hero => hero.IsAlive)
                                                       .ToArray();

        foreach (BattleHero hero in alivePlayerHeroes)
            hero.RewardHero();

        CurrentFightSettings.isRewardingComplete = true;

        while (true)
        {
            bool allReady = true;

            foreach (BattleHero hero in alivePlayerHeroes)
            {
                if (!hero.IsRewardingComplete)
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
                break;

            yield return null;
        }


        EndBattle();

    }

    IEnumerator DefatRoutine()
    {
        m_TurnText.text = DEFEAT_TEXT;

        CurrentFightSettings.isRewardingComplete = true;

        yield return null;

        EndBattle();
    }

    void OnVictory() =>
        StartCoroutine(VictoryRoutine());
    
    void OnDefeat() =>
        StartCoroutine(DefatRoutine());

    void EnemyPlay()
    {
        if (m_PlayerHeroes.Length <= 0) return;

        BattleHero enemy = null;
        if (CurrentFightSettings.selectedEnemyIndex == -1)
            enemy = GetAliveHero(m_EnemyHeroes);
        else
            enemy = FindBattleHeroOnIndex(m_EnemyHeroes, CurrentFightSettings.selectedEnemyIndex);

        CurrentFightSettings.selectedEnemyIndex = FindHeroIndex(m_EnemyHeroes, enemy);

        EventMessenger.NotifyEvent(SaveEvents.SAVE_GAME_STATE);

        enemy?.PerformAttack();
    }

    void UpdateTurnText()
    {
        if (m_AlivePlayerHeroCount == 0)
            m_TurnText.text = DEFEAT_TEXT;
        else if (m_AliveEnemyHeroCount == 0)
            m_TurnText.text = VICTORY_TEXT;
        else if (IsPlayerTurn)
            m_TurnText.text = PLAYER_TURN_TEXT;
        else
            m_TurnText.text = ENEMY_TURN_TEXT;
    }

    void AttackCompleted()
    {
        IsAttackInProgress = false;

        CurrentFightSettings.isPlayerTurn = !CurrentFightSettings.isPlayerTurn;
        IsPlayerTurn = CurrentFightSettings.isPlayerTurn;

        // Save Game State
        
        if(m_AlivePlayerHeroCount == 0)
            EventMessenger.NotifyEvent(FightEvents.ALL_ALLY_DEAD);
        else if(m_AliveEnemyHeroCount == 0)
            EventMessenger.NotifyEvent(FightEvents.ALL_ENEMY_DEAD);

        CurrentFightSettings.selectedAllyHeroIndex = -1;
        CurrentFightSettings.selectedEnemyIndex = -1;

        EventMessenger.NotifyEvent(SaveEvents.SAVE_GAME_STATE);

        if (m_AliveEnemyHeroCount != 0 &&
           m_AlivePlayerHeroCount != 0)
            UpdateTurnText();

        if (!IsPlayerTurn)
            EnemyPlay();
    }

    void AttackSignalGiven() =>
        IsAttackInProgress = true;

    void OnHeroDied(BattleHero hero)
    {
        if (hero.IsEnemy)
            --m_AliveEnemyHeroCount;
        else
            --m_AlivePlayerHeroCount;
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

        if (!hero.IsEnemy)
            CurrentFightSettings.selectedAllyHeroIndex = FindHeroIndex(m_PlayerHeroes, hero);

        EventMessenger.NotifyEvent(SaveEvents.SAVE_GAME_STATE);

        bool isDead = hero.TakeDamage(damage);
    }
}
