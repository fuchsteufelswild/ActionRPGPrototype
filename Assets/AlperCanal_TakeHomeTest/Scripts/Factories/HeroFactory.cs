using UnityEngine;

public static class HeroFactory
{
    static string[] HeroNameList = { "Vindicate", "Bionic", "Tornado", "Barrage", "Monsoon", "Glazier",
                                     "Barracuda", "Ember", "Tempest", "Velvet", "Nebula", "Licorice",
                                     "Iris", "Quartz", "Radiance", "Wildfire", "Bellona", "Anesthesia",
                                     "Voyd", "Mirage"};


    public struct HeroIngredients
    {
        public string heroName;
        public (int x, int y) healthRange;
        public (int x, int y) attackDamageRange;

        public HeroIngredients(string heroName, (int, int) healthRange, (int, int) attackDamageRange)
        {
            this.heroName = heroName;
            this.healthRange = healthRange;
            this.attackDamageRange = attackDamageRange;
        }
    }

    static string GetRandomName() =>
        HeroNameList[Random.Range(0, HeroNameList.Length)];

    static HeroData CreateHero(HeroIngredients data, bool isEnemy)
    {
        HeroData newHero = new HeroData();
        string heroName = GetRandomName();
        int health = Random.Range(data.healthRange.x, data.healthRange.y);
        int attackDamage = Random.Range(data.attackDamageRange.x, data.attackDamageRange.y);

        newHero.SetHeroData(heroName, health, attackDamage, isEnemy);
        return newHero;
    }

    public static HeroData CreateAllyHero(HeroIngredients data) =>
        CreateHero(data, true);

    public static HeroData CreateEnemyData(HeroIngredients data) =>
        CreateHero(data, false);




}
