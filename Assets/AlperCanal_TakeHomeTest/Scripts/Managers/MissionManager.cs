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
        public HeroData enemy = null;
    }

    public bool IsFighting => fightSettings.isFighting;
    public bool IsPlayerTurn => fightSettings.isPlayerTurn;

    FightSettings fightSettings;

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
