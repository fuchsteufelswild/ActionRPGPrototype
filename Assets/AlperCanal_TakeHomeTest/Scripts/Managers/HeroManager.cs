using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HeroManager : ManagerBase,
                           IGameManager
{
    List<int> m_OwnedHeroes;

    public int[] GetOwnedHeroes() => m_OwnedHeroes.ToArray();

    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IGameManager.Init()
    {
        m_Status = ManagerStatus.LOADING;

        yield return new WaitForSeconds(2f);

        Debug.Log("Hero Manager Started");

        m_Status = ManagerStatus.STARTED;
    }

    void IGameManager.UpdateData(object data)
    {
        throw new System.NotImplementedException();
    }
}
