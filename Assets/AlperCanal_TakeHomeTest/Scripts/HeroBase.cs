using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public abstract class HeroBase : MonoBehaviour,
                                 IPointerDownHandler,
                                 IPointerUpHandler
{
    protected HeroData m_Hero;

    [SerializeField] protected Image m_HeroFrame;
    [SerializeField] protected Image m_HeroImage;

    [SerializeField] protected Color m_DefaultFrameColor;
    [SerializeField] protected Color m_DefaultHeroImageColor;

    public string UITooltip => m_Hero.ToolTip();

    public void SetHeroData(HeroData heroData)
    {
        m_Hero = heroData;
        m_HeroImage.color = heroData.HeroColor;
        m_HeroFrame.color = m_DefaultFrameColor;
    }

    protected virtual void Start()
    {
        m_HeroImage.color = m_DefaultHeroImageColor;
        m_HeroFrame.color = m_DefaultFrameColor;
    }

    protected abstract void MakeSelection();

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (m_Hero == null) return;

        EventMessenger.NotifyEvent<HeroBase>(SelectionEvents.HERO_FRAME_CLICKED_DOWN, this);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (m_Hero == null) return;

        EventMessenger.NotifyEvent<Action>(SelectionEvents.HERO_FRAME_CLICKED_UP, MakeSelection);
    }

}
