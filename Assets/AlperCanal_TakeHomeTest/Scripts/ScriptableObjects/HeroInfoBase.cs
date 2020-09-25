using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Hero", menuName = "Create a Hero")]
public class HeroInfoBase : ScriptableObject
{
    public string heroName;
    public int attackDamage;
    public int baseHealth;

    public bool isEnemy;

    [TextArea] public string toolTip = HeroData.DefaultToolTip;

    public int HashCode() => heroName.GetHashCode();
    public string GetToolTip() => toolTip;

    static Dictionary<int, HeroInfoBase> cache;
    public static Dictionary<int, HeroInfoBase> data
    {
        get
        {
            if(cache == null)
            {
                HeroInfoBase[] heroes = Resources.LoadAll<HeroInfoBase>("Heroes/");

                cache = heroes.ToDictionary(hero => hero.GetHashCode(), hero => hero);
            }

            return cache;
        }
    }
}
