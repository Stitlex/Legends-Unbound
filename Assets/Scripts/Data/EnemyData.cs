using System;
using System.Collections.Generic;

[Serializable]
public class LootInfo
{
    public string weaponId;
    public float dropChance;
}

[Serializable]
public class EnemyInfo
{
    public string enemyId;
    public string enemyName;
    public int baseHealth;
    public int damage;
    public float speed;
    public int expReward;
    public int minGoldReward;
    public int maxGoldReward;
    public List<LootInfo> lootTable;
}

[Serializable]
public class EnemyDataContainer
{
    public List<EnemyInfo> enemies;
}