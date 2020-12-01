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

    void GameReset() =>
        StartCoroutine(StartUp());

    private void Awake()
    {
        managers = new List<IGameManager>();

        HeroManager = GetComponent<HeroManager>();
        MissionManager = GetComponent<MissionManager>();
        DataManager = GetComponent<DataManager>();
        AudioManager = GetComponentInChildren<AudioManager>();

        // Make sure data manager added last
        managers.Add(HeroManager);
        managers.Add(MissionManager);
        managers.Add(DataManager);

        StartCoroutine(StartUp());
    }

    private void Start()
    {
        EventMessenger.AddListener(SaveEvents.GAME_RESET, GameReset);
    }

    IEnumerator StartUp()
    {
        yield return new WaitForSeconds(0.1f);

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_STARTED);

        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        // Initialize everything but DataManager
        for (int i = 0; i < managers.Count - 1; ++i)
            StartCoroutine(managers[i].Init());

        int readyCount = 0;

        while(readyCount < managers.Count - 1)
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

        // Initialize DataManager last since it will set other managers up
        IGameManager dataMan = managers[managers.Count - 1];
        StartCoroutine(dataMan.Init());
        while (!dataMan.IsReady()) yield return waitForSeconds;

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_PROGRESS, 1, 1);

        EventMessenger.NotifyEvent(LoadingEvents.LOADING_FINISHED);
    }
}
