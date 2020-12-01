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

        /* Keeping track of enemy action
         * Could have kept player action
         * but this way player can restart
         * the game if he/she misclicks
         */ 
        public int selectedAllyHeroIndex = -1;
        public int selectedEnemyIndex = -1;

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

    public bool CanSelectHero => m_SelectedHeroes.Count < PLAYER_SIDE_COUNT;

    public int TotalPlayedFightCount { get; private set; }

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

    private void OnDestroy()
    {
        EventMessenger.RemoveListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, OnMenuHeroSelected);
        EventMessenger.RemoveListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, OnMenuHeroDeselected);
        EventMessenger.RemoveListener(SceneEvents.BATTLE_START_SIGNAL, OnBattleStart);
        EventMessenger.RemoveListener(SceneEvents.REWARDING_COMPLETE, LoadMenu);
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
        }

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, ++currentStep, stepCount);

        EventMessenger.NotifyEvent(SaveEvents.SAVE_GAME_STATE);
        
        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, ++currentStep, stepCount);
    }
    
    void CreateEnemyHeroes()
    {
        for (int i = 0; i < ENEMY_SIDE_COUNT; ++i)
            m_EnemyHeroes.Add(HeroFactory.GetNewEnemyHero());
    }

    void OnBattleStart() =>
        StartCoroutine(ConfigureBattle());

    // Ready the game state for battle scene
    IEnumerator ConfigureBattle()
    {
        int currentStep = 0;
        int stepCount = 3;
        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        CreateEnemyHeroes();
        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);

        fightSettings.UpdateSettings(true, true, false, m_SelectedHeroes.ToArray(), m_EnemyHeroes.ToArray());

        EventMessenger.NotifyEvent(SaveEvents.SAVE_GAME_STATE);
        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);

        m_BattleScene.PrepareBattleScene(fightSettings);
        yield return null;

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, currentStep++, stepCount);
        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);
    }

    /* Might have used anonymous functions when adding listener
     * but it is costly for anonymous function to keep private state
     */ 
    void OnMenuHeroSelected(HeroData hero) =>
        m_SelectedHeroes.Add(hero);

    void OnMenuHeroDeselected(HeroData hero) =>
        m_SelectedHeroes.Remove(hero);

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

    void IGameManager.UpdateData(object data)
    {
        if (data == null)
        {
            FindObjectOfType<UIController>().SetupUIForMenu();
            return;
        }

        fightSettings = data as FightSettings;

        if (fightSettings.isFighting)
        {
            // Setup the fight scene
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
