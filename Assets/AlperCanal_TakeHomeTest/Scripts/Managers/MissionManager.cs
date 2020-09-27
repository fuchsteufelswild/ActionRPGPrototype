using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : ManagerBase,
                              IGameManager
{
    const int REWARD_MATCH_COUNT = 1;

    [System.Serializable]
    public class FightSettings
    {
        public int consectuiveFightCount = 0;

        public bool isFighting = false;
        public bool isPlayerTurn = true;
        public bool isRewardingComplete = false;
        public HeroData[] playerSide = null;
        public HeroData[] enemySide = null;


        public void UpdateSettings(bool isFighting, bool isPlayerTurn, bool isRewardingComplete, HeroData[] playerSide, HeroData[] enemySide)
        {
            this.isFighting = isFighting;
            this.isPlayerTurn = isPlayerTurn;
            this.isRewardingComplete = isRewardingComplete;
            this.playerSide = playerSide;
            this.enemySide = enemySide;
        }

        public void Reset()
        {
            isFighting = false;
            isPlayerTurn = true;
            isRewardingComplete = false;
            playerSide = null;
            enemySide = null;
        }
    }

    public const int ENEMY_SIDE_COUNT = 1;
    public const int PLAYER_SIDE_COUNT = 3;

    public bool IsFighting => fightSettings.isFighting;
    public bool IsPlayerTurn => fightSettings.isPlayerTurn;

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
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, OnMenuHeroSelected);
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, OnMenuHeroDeselected);
        EventMessenger.AddListener(SceneEvents.BATTLE_START_SIGNAL, OnBattleStart);
        EventMessenger.AddListener(SceneEvents.REWARDING_COMPLETE, LoadMenu);
    }

    void LoadMenu()
    {
        int stepCount = 3;
        int currentStep = 0;

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        m_SelectedHeroes.Clear();
        m_EnemyHeroes.Clear();

        fightSettings.Reset();

        if (++fightSettings.consectuiveFightCount == REWARD_MATCH_COUNT)
        {
            fightSettings.consectuiveFightCount = 0;
            Managers.HeroManager.AddNewHero();

            // EventMessenger.NotifyEvent(SceneEvents.REWARD_MATCH_COUNT);
        }

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, ++currentStep, stepCount);

        // Save Here
        

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, ++currentStep, stepCount);
    }
    

    void OnBattleStart()
    {
        StartCoroutine(ConfigureBattle());
    }

    IEnumerator ConfigureBattle()
    {
        int currentStep = 0;
        int stepCount = 3;
        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        for (int i = 0; i < ENEMY_SIDE_COUNT; ++i)
            m_EnemyHeroes.Add(HeroFactory.GetNewEnemyHero());

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);

        fightSettings.playerSide = m_SelectedHeroes.ToArray();
        fightSettings.enemySide = m_EnemyHeroes.ToArray();
        fightSettings.isFighting = true;
        fightSettings.isPlayerTurn = true;
        fightSettings.isRewardingComplete = false;

        // Save Here    

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);

        m_BattleScene.PrepareBattleScene(fightSettings);
        // m_BattleScene.UpdateHeroes(fightSettings.playerSide, fightSettings.enemySide);

        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);

        yield return null;

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);

        
        m_BattleScene.CurrentFightSettings = fightSettings;
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
        if (data == null) return;

        fightSettings = data as FightSettings;

        if (fightSettings.isFighting)
        {
            // Important

            foreach (HeroData hero in fightSettings.playerSide)
                m_SelectedHeroes.Add(hero);

            foreach (HeroData hero in fightSettings.enemySide)
                m_EnemyHeroes.Add(hero);

            m_BattleScene.PrepareBattleScene(fightSettings);

            FindObjectOfType<UIController>().SetupUIForFight();
        }
        else
        {
            FindObjectOfType<UIController>().SetupUIForMenu();
        }

    }
}
