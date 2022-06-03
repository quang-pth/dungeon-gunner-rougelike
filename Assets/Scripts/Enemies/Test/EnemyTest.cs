using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplateSO = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateID);
        if (roomTemplateSO != null)
        {
            testLevelSpawnList = roomTemplateSO.enemiesByLevelList;
            randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailsSO enemyDetailsSO = randomEnemyHelperClass.GetItem();

            if (enemyDetailsSO != null)
            {
                instantiatedEnemyList.Add(Instantiate(enemyDetailsSO.enemyPrefab,
                    HelperUtilities.GetPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()), 
                    Quaternion.identity));
            }
        }
    }

}
