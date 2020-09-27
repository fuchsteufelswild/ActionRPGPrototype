using System.Collections;
using UnityEngine;
using System.IO;

public class DataManager : ManagerBase,
                           IGameManager
{
    string m_FilePath = "";

    ManagerStatus IGameManager.GetStatus() => base.GetStatus();

    bool IGameManager.IsReady() => base.IsReady();

    object IGameManager.GetData()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IGameManager.Init()
    {
        yield return new WaitForSeconds(0.1f);

        m_Status = ManagerStatus.LOADING;

        m_FilePath = Path.Combine(Application.persistentDataPath, "GameData.dat");

        LoadGameData();

        Debug.Log("Data Manager Started");

        m_Status = ManagerStatus.STARTED;
    }

    void IGameManager.UpdateData(object data)
    {
        throw new System.NotImplementedException();
    }

    public void SaveGameData()
    {
        // Only one save at a time

        // Will Save Manager Datas into a file
    }

    public void LoadGameData()
    {
        // Load Data From File and 
        // Update Managers

        if (!File.Exists(m_FilePath))
        {
            ((IGameManager)Managers.HeroManager).UpdateData(null);
        }
    }
}
