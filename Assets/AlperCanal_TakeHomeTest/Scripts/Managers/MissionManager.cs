using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : ManagerBase,
                              IGameManager
{
    [System.Serializable]
    public class FightSettings
    {
        public bool isFighting = false;
        public bool isPlayerTurn = false;
        public HeroData[] playerSide = null;
        public HeroData[] enemySide = null;
    }

    public const int ENEMY_SIDE_COUNT = 1;
    public const int PLAYER_SIDE_COUNT = 3;

    public bool IsFighting => fightSettings.isFighting;
    public bool IsPlayerTurn => fightSettings.isPlayerTurn;
    public bool IsAttackInProgress { get; set; }

    public bool CanSelectHero => m_SelectedHeroes.Count < 3;

    public List<HeroData> SelectedHeroes => m_SelectedHeroes;
    public HeroData[] SelectedHeroesArray =>
        m_SelectedHeroes == null ? null : m_SelectedHeroes.ToArray();

    FightSettings fightSettings;

    List<HeroData> m_SelectedHeroes;
    List<HeroData> m_EnemyHeroes;

    BattleScene m_BattleScene;
    private void Awake()
    {
        m_BattleScene = FindObjectOfType<BattleScene>();
        fightSettings = new FightSettings();
        IsAttackInProgress = false;
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, OnMenuHeroSelected);
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, OnMenuHeroDeselected);
        EventMessenger.AddListener(SceneEvents.BATTLE_START_SIGNAL, OnBattleStart);
    }

    void OnBattleStart()
    {
        StartCoroutine(ConfigureBattle());
    }

    IEnumerator ConfigureBattle()
    {
        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        for (int i = 0; i < ENEMY_SIDE_COUNT; ++i)
            m_EnemyHeroes.Add(HeroFactory.GetNewEnemyHero());

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, 1, 3);

        fightSettings.playerSide = m_SelectedHeroes.ToArray();
        fightSettings.enemySide = m_EnemyHeroes.ToArray();

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, 2, 3);

        m_BattleScene.UpdateHeroes(fightSettings.playerSide, fightSettings.enemySide);

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, 3, 3);

        yield return null;

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);

        
    }

    void OnMenuHeroSelected(HeroData hero)
    {
        m_SelectedHeroes.Add(hero);
    }

    void OnMenuHeroDeselected(HeroData hero)
    {
        m_SelectedHeroes.Remove(hero);
    }

    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData()
    {
        return fightSettings;
    }

    IEnumerator IGameManager.Init()
    {
        m_Status = ManagerStatus.LOADING;

        fightSettings = new FightSettings();
        m_SelectedHeroes = new List<HeroData>();
        m_EnemyHeroes = new List<HeroData>();

        Debug.Log("Mission Manager Started");

        yield return null;

        m_Status = ManagerStatus.STARTED;
    }

    void CreateObjects()
    {
        // Will create gameobjects for the fight scene
    }

    void PrepareFightScene()
    {
        // Update HeroData of gameobjects
    }

    void IGameManager.UpdateData(object data)
    {
        fightSettings = data as FightSettings;
    }
}
