using UnityEngine;

public class UIHero : HeroBase
{
    [SerializeField] Color m_SelectedFrameColor;

    bool isSelected = false;

    public void ResetSelection() => isSelected = false;

    public void ResetUIHero()
    {
        m_HeroFrame.color = m_DefaultFrameColor;
        m_HeroImage.color = m_DefaultHeroImageColor;
        m_Hero = null;
        ResetSelection();
    }

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
