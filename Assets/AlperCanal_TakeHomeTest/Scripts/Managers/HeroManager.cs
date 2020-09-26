using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class HeroManager : ManagerBase,
                           IGameManager
{
    public const int INITIAL_HERO_COUNT = 5;
    public const int MAX_AVAILABLE_HERO_COUNT = 10;

    List<HeroData> m_OwnedHeroes;

    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData() => m_OwnedHeroes;

    public List<HeroData> HeroData => m_OwnedHeroes;

    public HeroData[] HeroDataArray => m_OwnedHeroes.ToArray();

    IEnumerator IGameManager.Init()
    {
        m_OwnedHeroes = new List<HeroData>();

        m_Status = ManagerStatus.LOADING;       

        Debug.Log("Hero Manager Started");

        yield return null;

        m_Status = ManagerStatus.STARTED;
    }

    void CreateInitialHeroes()
    {
        for (int i = 0; i < INITIAL_HERO_COUNT; ++i)
            m_OwnedHeroes.Add(HeroFactory.GetNewAllyHero());

        EventMessenger.NotifyEvent(HeroEvents.HERO_ADDED);
    }

    void IGameManager.UpdateData(object data)
    {
        if (data == null)
        {
            CreateInitialHeroes();
            return;
        }
    
        m_OwnedHeroes = data as List<HeroData>;
            
    }
   
}
