using UnityEngine;
using UnityEditor;

public class HeroCreatorWizard : ScriptableWizard
{
    [MenuItem("MyTools/Create Hero")]
    public static void OpenWizard() => ScriptableWizard.DisplayWizard<HeroCreatorWizard>("Hero Creator", "Create Ally", "Create Enemy");

    public string heroName = "Hero Name";
    public int attackDamage = 10;
    public int baseHealth = 100;
    public Color heroColor;
    [HideInInspector] public string toolTip = HeroData.DefaultToolTip;

    private void CreateHero(bool isEnemy)
    {
        HeroInfoBase heroInfo = ScriptableObject.CreateInstance<HeroInfoBase>();
        
        heroInfo.heroName = heroName;
        heroInfo.attackDamage = attackDamage;
        heroInfo.baseHealth = baseHealth;
        heroInfo.toolTip = toolTip;
        heroInfo.heroColor = heroColor;

        heroInfo.isEnemy = isEnemy;

        SaveHero(heroInfo);
    }

    private void SaveHero(HeroInfoBase obj)
    {
        AssetDatabase.CreateAsset(obj, $"Assets/AlperCanal_TakeHomeTest/Resources/Heroes/{obj.heroName}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = obj;
    }

    private void OnWizardCreate() => CreateHero(false);

    private void OnWizardOtherButton() => CreateHero(true);

}
