using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UISelectionController : MonoBehaviour
{
    [SerializeField] GameObject m_ToolTip;

    IEnumerator selectionCoroutine = null;

    [SerializeField] float m_TimeForInformationReveal;

    float m_OffsetMultiplier = .5f;

    bool isShowingTooltip = false;
    private void Awake()
    {
        EventMessenger.AddListener<HeroBase>(SelectionEvents.HERO_FRAME_CLICKED_DOWN, OnHeroFrameClicked);
        EventMessenger.AddListener<Action>(SelectionEvents.HERO_FRAME_CLICKED_UP, OnHeroFrameMouseUp);        
    }

    Vector2 GetTooltipPosition(Vector2 screenPosition, RectTransform targetTransform) =>
        screenPosition + new Vector2(m_OffsetMultiplier * Screen.width / targetTransform.sizeDelta.x, 
                                     m_OffsetMultiplier * Screen.height / targetTransform.sizeDelta.y);

    void ShowTooltip(HeroBase target)
    {
        isShowingTooltip = true;

        Vector2 pos = GetTooltipPosition(target.transform.position, target.GetComponent<RectTransform>());

        m_ToolTip.transform.position = pos;
        m_ToolTip.GetComponentInChildren<UnityEngine.UI.Text>().text = target.UITooltip;

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
