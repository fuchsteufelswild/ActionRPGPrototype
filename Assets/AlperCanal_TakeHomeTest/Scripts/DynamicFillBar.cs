using UnityEngine;
using UnityEngine.UI;

public class DynamicFillBar : MonoBehaviour
{
    [SerializeField] Image m_FillBar;

    public void SetFillAmount(float amount) =>
        m_FillBar.fillAmount = amount;
}
