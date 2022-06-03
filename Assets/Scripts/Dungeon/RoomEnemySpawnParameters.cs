using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    #region Tooltip
    [Tooltip("Defines the dungeon level for this room with regard to how many enemies in total should be spawned")]
    #endregion
    public DungeonLevelSO dungeonLevelSO;

    #region Tooltip
    [Tooltip("The minimum number of enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int minTotalEnemiesToSpawn;

    #region Tooltip
    [Tooltip("The maximum number of enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int maxTotalEnemiesToSpawn;

    #region Tooltip
    [Tooltip("The minimum number of concurrent enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int minConcurrentEnemies;

    #region Tooltip
    [Tooltip("The maximum number of concurrent enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int maxConcurrentEnemies;
    
    #region Tooltip
    [Tooltip("The minimum spawn intervals in seconds for enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int minSpawnInterval;
    
    #region Tooltip
    [Tooltip("The maximum spawn intervals in seconds for enemies to be spawn in this room. The actual number will be a random between the minimum and maximum")]
    #endregion
    public int maxSpawnInterval;
}
