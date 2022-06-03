using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomSpawnableObject<T>
{
    private struct ChanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueTotal = 0;
    private List<ChanceBoundaries> chanceBoundariesList = new List<ChanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        int upperBoundary = -1;
        ratioValueTotal = 0;
        T spawnableObject = default(T);

        foreach(SpawnableObjectsByLevel<T> spawnableObjectsByLevel in spawnableObjectsByLevelList)
        {
            if (spawnableObjectsByLevel.dungeonLevelSO == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach(SpawnableObjectRatio<T> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                {
                    int lowerBoundary = upperBoundary + 1;
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;
                    ratioValueTotal += spawnableObjectRatio.ratio;

                    // Create new object instace
                    ChanceBoundaries chanceBoundaryObject = new ChanceBoundaries()
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    };
                    chanceBoundariesList.Add(chanceBoundaryObject);
                }
            }
        }

        if (chanceBoundariesList.Count == 0) return default(T);

        int lookupValue = Random.Range(0, ratioValueTotal);
        foreach(ChanceBoundaries spawnChance in chanceBoundariesList)
        {
            if (lookupValue >= spawnChance.lowBoundaryValue && lookupValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}
