using UnityEngine;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    [SerializeField] Image m_Background;
    [SerializeField] DynamicFillBar m_LoadingBar;

    private void Awake()
    {
        EventMessenger.AddListener(LoadingEvents.LOADING_STARTED, StartLoading);
        EventMessenger.AddListener(LoadingEvents.LOADING_FINISHED, LoadingFinished);
        EventMessenger.AddListener<int, int>(LoadingEvents.LOADING_PROGRESS, UpdateLoadingBar);
    }

    private void OnDestroy()
    {
        EventMessenger.RemoveListener(LoadingEvents.LOADING_STARTED, StartLoading);
        EventMessenger.RemoveListener(LoadingEvents.LOADING_FINISHED, LoadingFinished);
        EventMessenger.RemoveListener<int, int>(LoadingEvents.LOADING_PROGRESS, UpdateLoadingBar);
    }

    void StartLoading()
    {
        m_LoadingBar.SetFillAmount(0);
        m_Background.gameObject.SetActive(true);
        m_LoadingBar.gameObject.SetActive(true);
    }

    void LoadingFinished()
    {
        m_Background.gameObject.SetActive(false);
        m_LoadingBar.gameObject.SetActive(false);
    }

    void UpdateLoadingBar(int numberReady, int totalNumber) =>
        m_LoadingBar.SetFillAmount(numberReady / (float)totalNumber);
}
