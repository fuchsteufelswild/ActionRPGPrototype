using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject m_UIHeroPrefab;

    [SerializeField] GameObject m_GameMenu;
    [SerializeField] GameObject m_HeroLayout;
    [SerializeField] GameObject m_BattleScene;
    [SerializeField] Button m_BattleButton;

    List<UIHero> uiHeros;

    void Start()
    {
        uiHeros = new List<UIHero>();

        for(int i = 0; i < HeroManager.MAX_AVAILABLE_HERO_COUNT; ++i)
            uiHeros.Add(Instantiate(m_UIHeroPrefab, m_HeroLayout.transform).GetComponent<UIHero>());

        EventMessenger.AddListener(HeroEvents.HERO_ADDED, Refresh);
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, OnHeroSelected);
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, OnHeroDeselected);
        EventMessenger.AddListener(SceneEvents.REWARDING_COMPLETE, LoadMenu);
    }

    void LoadMenu()
    {
        SetupUIForMenu();

        EventMessenger.NotifyEvent<int, int>(LoadingEvents.LOADING_PROGRESS, 1, 1);
        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);
    }

    public void SignalBattle()
    {
        EventMessenger.NotifyEvent(SceneEvents.BATTLE_START_SIGNAL);

        SetupUIForFight();
    }

    public void SetupUIForFight()
    {
        m_GameMenu.gameObject.SetActive(false);
        m_BattleScene.gameObject.SetActive(true);
    }

    public void SetupUIForMenu()
    {
        Refresh();

        m_BattleButton.interactable = false;
        m_GameMenu.gameObject.SetActive(true);
        m_BattleScene.gameObject.SetActive(false);
    }

    void OnHeroSelected(HeroData hero)
    {
        if (!Managers.MissionManager.CanSelectHero)
            m_BattleButton.interactable = true;
    }

    void OnHeroDeselected(HeroData hero)
    {
        m_BattleButton.interactable = false;
    }

    public void Refresh()
    {
        HeroData[] heroDatas = Managers.HeroManager.HeroDataArray;

        for (int i = 0; i < heroDatas.Length; ++i)
        {
            heroDatas[i].ResetHealth();
            uiHeros[i].SetHeroData(heroDatas[i]);
            uiHeros[i].ResetSelection();
        }
    }

}
