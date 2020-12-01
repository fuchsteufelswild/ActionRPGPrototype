using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    Animator m_Animator;
    RectTransform m_RectTransform;

    public void RegulateSize(RectTransform target) =>
        m_RectTransform.sizeDelta = target.sizeDelta;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(m_Animator != null) m_Animator.SetTrigger("PlayAttackAnimation");
    }

    void OnAnimationFinished() =>
        gameObject.SetActive(false);

}
