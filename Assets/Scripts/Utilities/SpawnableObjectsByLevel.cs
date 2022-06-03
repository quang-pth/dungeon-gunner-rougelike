using System.Collections.Generic;

[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    public DungeonLevelSO dungeonLevelSO;
    public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
}
