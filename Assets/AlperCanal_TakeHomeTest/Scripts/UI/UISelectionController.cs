using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class UISelectionController : MonoBehaviour
{
    [SerializeField] GameObject m_ToolTip;

    [SerializeField] float m_TimeForInformationReveal;

    IEnumerator selectionCoroutine = null;

    float m_OffsetMultiplier = .2f;

    bool isShowingTooltip = false;
    private void Awake()
    {
        EventMessenger.AddListener<HeroBase>(SelectionEvents.HERO_FRAME_CLICKED_DOWN, OnHeroFrameClicked);
        EventMessenger.AddListener<Action>(SelectionEvents.HERO_FRAME_CLICKED_UP, OnHeroFrameMouseUp);        
    }

    private void OnDestroy()
    {
        EventMessenger.RemoveListener<HeroBase>(SelectionEvents.HERO_FRAME_CLICKED_DOWN, OnHeroFrameClicked);
        EventMessenger.RemoveListener<Action>(SelectionEvents.HERO_FRAME_CLICKED_UP, OnHeroFrameMouseUp);
    }

    Vector2 GetTooltipPosition(Vector2 screenPosition, RectTransform targetTransform) =>
        screenPosition + new Vector2(m_OffsetMultiplier * (Screen.width / 1920f) * targetTransform.sizeDelta.x, 
                                     m_OffsetMultiplier * (Screen.height / 1080f) * targetTransform.sizeDelta.y);

    void ShowTooltip(HeroBase target)
    {
        isShowingTooltip = true;
        
        m_ToolTip.transform.position = target.transform.position;
        m_ToolTip.GetComponentInChildren<TextMeshProUGUI>().text = target.UITooltip;

        m_ToolTip.SetActive(true);
    }

    void HideTooltip()
    {
        isShowingTooltip = false;
        m_ToolTip.SetActive(false);
    }

    void OnHeroFrameClicked(HeroBase target)
    {
        if (selectionCoroutine != null)
            StopCoroutine(selectionCoroutine);

        selectionCoroutine = SelectionRoutine(target);

        StartCoroutine(selectionCoroutine);
    }

    void OnHeroFrameMouseUp(Action callback)
    {
        if (!isShowingTooltip)
        {
            StopCoroutine(selectionCoroutine);
            callback();
        }

        HideTooltip();
    }

    IEnumerator SelectionRoutine(HeroBase target)
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        float timePassed = 0.0f;

        while(timePassed <= m_TimeForInformationReveal)
        {
            timePassed += Time.deltaTime;

            yield return waitForEndOfFrame;
        }

        ShowTooltip(target);
    }
}
