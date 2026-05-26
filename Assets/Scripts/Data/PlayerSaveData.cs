using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public int currentHealth;
    public string equippedMeleeId;
    public string equippedRangedId;


    public int level;
    public int currentExp;
    public int expToNextLevel;

    public int strength;
    public int agility;

    public int gold;
    public List<string> unlockedWeapons;
    public List<string> unlockedFood;
}