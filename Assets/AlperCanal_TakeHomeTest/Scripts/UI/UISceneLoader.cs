using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    [SerializeField] Image m_Background;
    [SerializeField] Image m_LoadingBar;

    private void Awake()
    {
        EventMessenger.AddListener(LoadingEvents.LOADING_STARTED, StartLoading);
        EventMessenger.AddListener(LoadingEvents.LOADING_FINISHED, LoadingFinished);
        EventMessenger.AddListener<int, int>(LoadingEvents.LOADING_PROGRESS, UpdateSlider);
    }

    private void OnDestroy()
    {
        EventMessenger.RemoveListener(LoadingEvents.LOADING_STARTED, StartLoading);
        EventMessenger.RemoveListener(LoadingEvents.LOADING_FINISHED, LoadingFinished);
        EventMessenger.RemoveListener<int, int>(LoadingEvents.LOADING_PROGRESS, UpdateSlider);
    }

    public void StartLoading()
    {
        m_Background.gameObject.SetActive(true);
        m_LoadingBar.gameObject.SetActive(true);

        m_LoadingBar.fillAmount = 0;
    }

    public void LoadingFinished()
    {
        m_Background.gameObject.SetActive(false);
        m_LoadingBar.gameObject.SetActive(false);
    }

    public void UpdateSlider(int numberReady, int totalNumber) =>
        m_LoadingBar.fillAmount = (float)numberReady / totalNumber;
}
