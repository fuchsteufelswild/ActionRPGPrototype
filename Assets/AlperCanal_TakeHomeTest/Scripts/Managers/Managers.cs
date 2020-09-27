/* Main class for the initialization 
 * Starts all the managers up
 */ 

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DataManager), 
                  typeof(MissionManager), 
                  typeof(HeroManager))]
public class Managers : MonoBehaviour
{
    public static DataManager DataManager;
    public static MissionManager MissionManager;
    public static HeroManager HeroManager;
    public static AudioManager AudioManager;

    List<IGameManager> managers;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        managers = new List<IGameManager>();

        HeroManager = GetComponent<HeroManager>();
        MissionManager = GetComponent<MissionManager>();
        DataManager = GetComponent<DataManager>();
        AudioManager = GetComponentInChildren<AudioManager>();

        managers.Add(HeroManager);
        managers.Add(MissionManager);
        managers.Add(DataManager);

        StartCoroutine(StartUp());
    }

    IEnumerator StartUp()
    {
        yield return new WaitForSeconds(0.1f);

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        foreach (IGameManager gameManager in managers)
            StartCoroutine(gameManager.Init());

        int readyCount = 0;

        while(readyCount < managers.Count)
        {
            int currentReadyCount = 0;

            foreach (IGameManager gameManager in managers)
                if (gameManager.IsReady())
                    ++currentReadyCount;

            if(currentReadyCount > readyCount)
            {
                EventMessenger.NotifyEvent(LoadingEvents.LOADING_PROGRESS, currentReadyCount, managers.Count);

                readyCount = currentReadyCount;
            }

            yield return waitForSeconds;
        }

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);
    }
}
