using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HeroCreatorWizard : ScriptableWizard
{
    [MenuItem("MyTools/Create Hero")]
    public static void OpenWizard() => ScriptableWizard.DisplayWizard<HeroCreatorWizard>("Hero Creator", "Create Ally", "Create Enemy");

    public string heroName = "Hero Name";
    public int attackDamage = 10;
    public int baseHealth = 100;
    private bool isEnemy = false;
    [TextArea] public string toolTip = "";

    private HeroInfoBase CreateHero()
    {
        HeroInfoBase heroInfoBase = ScriptableObject.CreateInstance<HeroInfoBase>();

        heroInfoBase.heroName = heroName;
        heroInfoBase.attackDamage = attackDamage;
        heroInfoBase.baseHealth = baseHealth;
        heroInfoBase.toolTip = toolTip;

        return heroInfoBase;
    }

    private void SaveHero(HeroInfoBase obj)
    {
        AssetDatabase.CreateAsset(obj, $"Assets/AlperCanal_TakeHomeTest/Resources/Heroes/{obj.heroName}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = obj;
    }

    private void OnWizardCreate()
    {
        HeroInfoBase obj = CreateHero();
        obj.isEnemy = false;

        SaveHero(obj);
    }

    private void OnWizardOtherButton()
    {
        HeroInfoBase obj = CreateHero();
        obj.isEnemy = true;

        SaveHero(obj);
    }

}
