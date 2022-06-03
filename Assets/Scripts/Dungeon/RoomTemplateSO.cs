using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ROOM PREFAB")]

    #endregion Header ROOM PREFAB

    #region Tooltip

    [Tooltip("The gameobject prefab for the room (this will contain all the tilemaps for the room and environment game objects")]

    #endregion Tooltip

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the guid if the so is copied and the prefab is changed


    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("ROOM CONFIGURATION")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip("The room node type SO. The room node types correspond to the room nodes used in the room node graph.  The exceptions being with corridors.  In the room node graph there is just one corridor type 'Corridor'.  For the room templates there are 2 corridor node types - CorridorNS and CorridorEW.")]

    #endregion Tooltip
    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room lower bounds represent the bottom left corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that bottom left corner (Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip("If you imagine a rectangle around the room tilemap that just completely encloses it, the room upper bounds represent the top right corner of that rectangle. This should be determined from the tilemap for the room (using the coordinate brush pointer to get the tilemap grid position for that top right corner (Note: this is the local tilemap position and NOT world position")]

    #endregion Tooltip
    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip("There should be a maximum of four doorways for a room - one for each compass direction.  These should have a consistent 3 tile opening size, with the middle tile position being the doorway coordinate 'position'")]

    #endregion Tooltip
    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip

    [Tooltip("Each possible spawn position (used for enemies and chests) for the room in tilemap coordinates should be added to this array")]

    #endregion Tooltip
    public Vector2Int[] spawnPositionArray;

    #region Header ENEMY DETAILS
    [Space(10)]
    [Header("ENEMY DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate the list with all the enemies that can be spawned in this room by dungeon level list" +
        ", including the random ratio of this enemy type that will be spawned")]
    #endregion
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    #region Tooltip
    [Tooltip("Populate the list with the enemy spawn parameters")]
    #endregion
    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // Validate SO fields
    private void OnValidate()
    {
        // Set unique GUID if empty or the prefab changes
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);


        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {   
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);
            
            foreach (RoomEnemySpawnParameters spawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(spawnParameters.dungeonLevelSO), spawnParameters.dungeonLevelSO);
                // number of enemies to spawn
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(spawnParameters.minTotalEnemiesToSpawn), spawnParameters.minTotalEnemiesToSpawn,
                    nameof(spawnParameters.maxTotalEnemiesToSpawn), spawnParameters.maxTotalEnemiesToSpawn, true);
                // spawn interval
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(spawnParameters.minSpawnInterval), spawnParameters.minSpawnInterval,
                    nameof(spawnParameters.maxSpawnInterval), spawnParameters.maxSpawnInterval, true);
                // concurrent enemies
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(spawnParameters.minConcurrentEnemies), spawnParameters.minConcurrentEnemies,
                    nameof(spawnParameters.maxConcurrentEnemies), spawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesForDungeonLevel = false;

                foreach(SpawnableObjectsByLevel<EnemyDetailsSO> spawnableObjectsByLevel in enemiesByLevelList)
                {
                    if (spawnableObjectsByLevel.dungeonLevelSO == spawnParameters.dungeonLevelSO && spawnableObjectsByLevel.spawnableObjectRatioList.Count > 0)
                    {
                        isEnemyTypesForDungeonLevel = true;
                    }

                    HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectsByLevel.dungeonLevelSO), spawnableObjectsByLevel.dungeonLevelSO);

                    foreach(SpawnableObjectRatio<EnemyDetailsSO> spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(spawnableObjectRatio.dungeonObject), spawnableObjectRatio.dungeonObject);
                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(spawnableObjectRatio.ratio), spawnableObjectRatio.ratio, false);
                    }

                    if (isEnemyTypesForDungeonLevel == false && spawnParameters.dungeonLevelSO != null)
                    {
                        Debug.Log("No enemy types specified for the dungeon level " + spawnParameters.dungeonLevelSO.name +
                            " in game object " + this.name.ToString());
                    }
                }
            }
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}