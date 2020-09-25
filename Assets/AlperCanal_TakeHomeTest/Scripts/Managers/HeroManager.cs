using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HeroManager : ManagerBase,
                           IGameManager
{
    // Dictionary<int, HeroData> m_OwnedHeroes;
    List<HeroData> m_OwnedHeroes;

    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData() => m_OwnedHeroes;

    IEnumerator IGameManager.Init()
    {
        m_OwnedHeroes = new List<HeroData>();

        m_Status = ManagerStatus.LOADING;       

        Debug.Log("Hero Manager Started");

        foreach (KeyValuePair<int, HeroInfoBase> pair in HeroInfoBase.data)
            Debug.Log($"{pair.Value.GetToolTip()}");

        yield return null;

        m_Status = ManagerStatus.STARTED;
    }

    void IGameManager.UpdateData(object data)
    {
        m_OwnedHeroes = data as List<HeroData>;
    }
}
