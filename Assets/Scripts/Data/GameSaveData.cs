using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public PlayerSaveData playerData = new PlayerSaveData();
    public List<EnemySaveData> enemiesData = new List<EnemySaveData>();
}