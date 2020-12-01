using System.Collections;
using UnityEngine;
using TMPro;

public class AttributeChange : MonoBehaviour
{
    public const string HEALTH_DECREASE_TEXT = "HEALTH -{DECREASE}";
    public const string HEALTH_INCREASE_TEXT = "MAX HEALTH +{INCREASE}";
    public const string EXPERIENCE_INCREASE_TEXT = "EXPERIENCE +{INCREASE}";
    public const string ATTACK_DAMAGE_INCREASE_TEXT = "ATTACK DAMAGE +{INCREASE}";
    public const string LEVEL_INCREASE_TEXT = "LEVEL +{INCREASE}";

    [SerializeField] TextMeshProUGUI m_NotificationText;

    [SerializeField] float m_FadeoutTime;
    [SerializeField] Color m_PositiveEffectColor;
    [SerializeField] Color m_NegativeEffectColor;

    bool isInitialized = false;

    bool isPositiveEffect;
    RectTransform m_RectTransform;
    Vector3 m_StartPosition;
    Vector3 m_EndPosition;

    IEnumerator notificationRoutine;
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    float m_OffsetMultiplier = 0.0005f;
    public void PrepareForActivation(Transform targetTransform, bool isPositiveEffect, string displayText)
    {
        this.isPositiveEffect = isPositiveEffect;

        SetPositionAndText(targetTransform, displayText);
    }

    void SetPositionAndText(Transform targetTransform, string displayText)
    {
        m_StartPosition = targetTransform.position;

        Vector2 targetSize = targetTransform.GetComponent<RectTransform>().sizeDelta;

        m_RectTransform.sizeDelta = targetSize * .8f;
        m_EndPosition = m_StartPosition + new Vector3(0, Screen.height * targetSize.y * m_OffsetMultiplier, 0);

        m_NotificationText.text = displayText;
    }

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        SetPositionAndText(transform, "");
        notificationRoutine = NotificationRoutine();
    }

    private void OnEnable()
    {
        if(!isInitialized)
        {
            isInitialized = true;
            return;
        }

        if (notificationRoutine != null)
            StopCoroutine(notificationRoutine);

        notificationRoutine = NotificationRoutine();

        transform.position = m_StartPosition;

        StartCoroutine(notificationRoutine);
    }

    void Deactivate() =>
        gameObject.SetActive(false);

    IEnumerator NotificationRoutine()
    {
        float passedTime = 0.0f;

        Color textColor = Color.white;

        if (isPositiveEffect) textColor = m_PositiveEffectColor;
        else textColor = m_NegativeEffectColor;

        while(passedTime < m_FadeoutTime)
        {
            transform.position = Vector3.Lerp(transform.position, m_EndPosition, (passedTime / m_FadeoutTime));
            
            textColor.a = 1 - (passedTime / m_FadeoutTime);
            m_NotificationText.color = textColor;

            passedTime += Time.deltaTime;

            yield return waitForEndOfFrame;
        }   

        Invoke("Deactivate", 0.1f);
    }

}
