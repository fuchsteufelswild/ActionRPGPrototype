using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip m_ButtonClip;
    [SerializeField] AudioClip m_SelectionClip;
    [SerializeField] AudioClip m_AttackClip;

    AudioSource m_AudioSource;
    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, PlaySelectionSound);
        EventMessenger.AddListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, PlaySelectionSound);

        EventMessenger.AddListener<BattleHero, int>(FightEvents.DAMAGE_DONE, PlayAttackSound);
        EventMessenger.AddListener(SceneEvents.BATTLE_START_SIGNAL, PlayButtonSound);
    }

    private void OnDestroy()
    {
        EventMessenger.RemoveListener<HeroData>(SelectionEvents.HERO_FRAME_SELECTED, PlaySelectionSound);
        EventMessenger.RemoveListener<HeroData>(SelectionEvents.HERO_FRAME_DESELECTED, PlaySelectionSound);

        EventMessenger.RemoveListener<BattleHero, int>(FightEvents.DAMAGE_DONE, PlayAttackSound);
        EventMessenger.RemoveListener(SceneEvents.BATTLE_START_SIGNAL, PlayButtonSound);
    }

    void PlayAttackSound(BattleHero hero = null, int damage = -1) => m_AudioSource.PlayOneShot(m_AttackClip);
    void PlaySelectionSound(HeroData data = null) => m_AudioSource.PlayOneShot(m_SelectionClip);
    void PlayButtonSound() => m_AudioSource.PlayOneShot(m_ButtonClip);
}
