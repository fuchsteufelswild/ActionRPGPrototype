using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : ManagerBase,
                           IGameManager
{
    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IGameManager.Init()
    {
        m_Status = ManagerStatus.LOADING;

        yield return new WaitForSeconds(6f);
        Debug.Log("Data Manager Started");
        m_Status = ManagerStatus.STARTED;
    }

    void IGameManager.UpdateData(object data)
    {
        throw new System.NotImplementedException();
    }
}
