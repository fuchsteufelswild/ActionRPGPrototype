using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Button newButton;

    private void Awake()
    {
        newButton.onClick.AddListener(() => Managers.HeroManager.AddNewHero());
    }
}
