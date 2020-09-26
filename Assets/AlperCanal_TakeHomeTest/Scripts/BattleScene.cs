using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BattleScene : MonoBehaviour
{
    [SerializeField] BattleHero[] m_PlayerHeroes;
    [SerializeField] BattleHero[] m_EnemyHeroes;

    public void UpdateHeroes(HeroData[] playerHeroes, HeroData[] enemyHeroes)
    {
        for (int i = 0; i < playerHeroes.Length; ++i)
            m_PlayerHeroes[i].SetHeroData(playerHeroes[i]);

        for (int i = 0; i < enemyHeroes.Length; ++i)
            m_EnemyHeroes[i].SetHeroData(enemyHeroes[i]);
    }

    BattleHero GetHeroWithCondition(BattleHero[] battleHeroes, Func<BattleHero, bool> predicate)
    {
        BattleHero[] aliveHeroes = battleHeroes.Where(hero => predicate(hero))
                                               .ToArray();

        if (aliveHeroes.Length > 0)
            return aliveHeroes[UnityEngine.Random.Range(0, aliveHeroes.Length)];

        return null;
    }

    BattleHero GetAliveHero(BattleHero[] battleHeroes) =>
        GetHeroWithCondition(battleHeroes, hero => hero.IsAlive);
    
    public BattleHero GetRandomAliveHero(bool isEnemy)
    {
        if (isEnemy) return GetAliveHero(m_EnemyHeroes);
        return GetAliveHero(m_PlayerHeroes);
    }

    private void Start()
    {
        Debug.Assert(m_PlayerHeroes.Length >= MissionManager.PLAYER_SIDE_COUNT &&
                     m_EnemyHeroes.Length >= MissionManager.ENEMY_SIDE_COUNT);
    }
}
