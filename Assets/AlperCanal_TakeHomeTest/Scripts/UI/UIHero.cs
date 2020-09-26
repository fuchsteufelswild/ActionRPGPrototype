using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UIHero : HeroBase
{
    [SerializeField] Color m_SelectedFrameColor;

    bool isSelected = false;

    protected override void MakeSelection()
    {
        if (isSelected)
        {
            m_HeroFrame.color = m_DefaultFrameColor;
            EventMessenger.NotifyEvent<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, m_Hero);
        }
        else if (Managers.MissionManager.CanSelectHero)
        {
            m_HeroFrame.color = m_SelectedFrameColor;
            EventMessenger.NotifyEvent<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, m_Hero);
        }
        else
            return;
        
        isSelected = !isSelected;
    }
}
