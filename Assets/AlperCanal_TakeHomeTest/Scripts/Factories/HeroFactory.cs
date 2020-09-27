using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public static class HeroFactory
{
    public struct HeroIngredients
    {
        public (int x, int y) healthRange;
        public (int x, int y) attackDamageRange;

        public HeroIngredients((int, int) healthRange, (int, int) attackDamageRange)
        {
            this.healthRange = healthRange;
            this.attackDamageRange = attackDamageRange;
        }
    }

    public const int ALLY_HERO_MIN_HEALTH_LIMIT = 100;
    public const int ALLY_HERO_MAX_HEALTH_LIMIT = 200;

    public const int ALLY_HERO_MIN_ATTACK_LIMIT = 10;
    public const int ALLY_HERO_MAX_ATTACK_LIMIT = 20;

    public const int ENEMY_HERO_MIN_HEALTH_LIMIT = 20;
    public const int ENEMY_HERO_MAX_HEALTH_LIMIT = 25;

    public const int ENEMY_HERO_MIN_ATTACK_LIMIT = 200;
    public const int ENEMY_HERO_MAX_ATTACK_LIMIT = 400;

    static string[] HeroNameList = { "Vindicate", "Bionic", "Tornado", "Barrage", "Monsoon", "Glazier",
                                     "Barracuda", "Ember", "Tempest", "Velvet", "Nebula", "Licorice",
                                     "Iris", "Quartz", "Radiance", "Wildfire", "Bellona", "Anesthesia",
                                     "Voyd", "Mirage"};

    /* Might read from a file or from a database
     * but preferred more simplistic approach
     */
    static Dictionary<string, Color> availableHeroNamesAndColors = new Dictionary<string, Color>()
    {
        { "Vindicate", Color.black },
        { "Bionic", Color.green },
        { "Tornado", Color.grey },
        { "Barrage", Color.yellow },
        { "Glazier", Color.blue },
        { "Iris", Color.magenta },
        { "Ember", Color.red },
        { "Bellona", Color.white },
        { "Mirage", Color.cyan },
        { "Monsoon", (Color.yellow + Color.red) / 2f },
        { "Velvet", (Color.blue + Color.magenta) / 2f}
    };

    public static void UpdateAvailableHeros()
    {
        List<HeroData> heroes = Managers.HeroManager.HeroData;

        foreach (HeroData hero in heroes)
            availableHeroNamesAndColors.Remove(hero.HeroName);
    }

    static KeyValuePair<string, Color> GetRandomName() =>
        availableHeroNamesAndColors.ElementAt(Random.Range(0, availableHeroNamesAndColors.Count));

    // Create a Hero with random attributes
    static HeroData CreateHero(HeroIngredients data, bool isEnemy)
    {
        HeroData newHero = new HeroData();
        KeyValuePair<string, Color> heroAttributes = GetRandomName();
        int health = Random.Range(data.healthRange.x, data.healthRange.y + 1);
        int attackDamage = Random.Range(data.attackDamageRange.x, data.attackDamageRange.y + 1);

        newHero.SetHeroData(heroAttributes, health, attackDamage, isEnemy);

        if(!isEnemy)
            availableHeroNamesAndColors.Remove(heroAttributes.Key);

        return newHero;
    }

    static HeroData CreateAllyHero(HeroIngredients data) =>
        CreateHero(data, false);

    static HeroData CreateEnemyHero(HeroIngredients data) =>
        CreateHero(data, true);

    static HeroData CreateHeroFromScriptableObject(HeroInfoBase info) =>
        new HeroData(info);

    // Create a new hero from a recipe or using random attributes
    static HeroData GetNewHero(bool isEnemy)
    {
        var heroDataBase = HeroInfoBase.data.Where(pair => pair.Value.isEnemy == isEnemy)
                                            .ToDictionary(pair => pair.Key, pair => pair.Value);

        int randomIndex = Random.Range(0, HeroInfoBase.data.Count * 2);

        HeroData newHero = null;

        if (randomIndex >= heroDataBase.Count)
        {
            if (!isEnemy)
                newHero = CreateAllyHero(new HeroIngredients((ALLY_HERO_MIN_HEALTH_LIMIT, ALLY_HERO_MAX_HEALTH_LIMIT), 
                                                             (ALLY_HERO_MIN_ATTACK_LIMIT, ALLY_HERO_MAX_ATTACK_LIMIT)));
            else
                newHero = CreateEnemyHero(new HeroIngredients((ENEMY_HERO_MIN_HEALTH_LIMIT, ENEMY_HERO_MAX_HEALTH_LIMIT), 
                                                              (ENEMY_HERO_MIN_ATTACK_LIMIT, ENEMY_HERO_MAX_ATTACK_LIMIT)));
        }
        else
        {
            newHero = CreateHeroFromScriptableObject(heroDataBase.ElementAt(randomIndex).Value);

            HeroInfoBase.data.Remove(newHero.HashCode);
        }

        return newHero;
    }

    public static HeroData GetNewAllyHero() =>
        GetNewHero(false);

    public static HeroData GetNewEnemyHero() =>
        GetNewHero(true);
}
