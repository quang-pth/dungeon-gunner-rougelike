using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO dungeonLevelSO;
        [Range(0, 100)] public int min;
        [Range(0, 100)] public int max;
    }

    #region Header CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the chest prefab")]
    #endregion
    [SerializeField] private GameObject chestPrefab;

    #region Header CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion
    #region Tooltip
    [Tooltip("The minimum probability for spawning a chest")]
    #endregion
    [SerializeField, Range(0, 100)] private int chestSpawnChanceMin;
    #region Tooltip
    [Tooltip("The maximum probability for spawning a chest")]
    #endregion
    [SerializeField, Range(0, 100)] private int chestSpawnChanceMax;
    #region Tooltip
    [Tooltip("Override the chest spawn chance by dungeon level")]
    #endregion
    [SerializeField] private List<RangeByLevel> chestSpawnChanceByLevelList;

    #region Header CHEST SPAWN DETAILS
    [Space(10), Header("CHEST SPAWN DETAILS")]
    #endregion
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    #region Tooltip
    [Tooltip("The minimum number of items to spawn")]
    #endregion
    [SerializeField, Range(0, 3)] private int numberOfItemsToSpawnMin;
    #region Tooltip
    [Tooltip("The maximum number of items to spawn")]
    #endregion
    [SerializeField, Range(0, 3)] private int numberOfItemsToSpawnMax;

    #region Header CHEST CONTENT DETAILS
    [Space(10), Header("CHEST CONTENT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The weapons to spawn for each dungeon level and their spawn ratios")]
    #endregion
    [SerializeField] private List<SpawnableObjectsByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;
    #region Tooltip
    [Tooltip("The range of health to spawn for each level")]
    #endregion
    [SerializeField] private List<RangeByLevel> healthSpawnByLevelList;
    #region Tooltip
    [Tooltip("The range of ammo to spawn for each level")]
    #endregion
    [SerializeField] private List<RangeByLevel> ammoSpawnByLevelList;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
        {
            SpawnChest();
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs roomEnemiesDefeatedEventArgs)
    {
        if (chestRoom == null)
        {
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedEventArgs.room)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        chestSpawned = true;

        if (!RandomSpawnChest()) return;

        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            Vector3 spawnPosition = HelperUtilities.GetPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);
            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
            chestGameObject.transform.position = spawnPosition + variation;
        }

        Chest chest = chestGameObject.GetComponent<Chest>();
        bool shouldMaterialized = chestSpawnEvent == ChestSpawnEvent.onRoomEntry;
        chest.Intialize(shouldMaterialized, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum),
            GetAmmoPercentToSpawn(ammoNum));
    }

    private int GetAmmoPercentToSpawn(int ammoNum)
    {
        if (ammoNum == 0) return 0;

        foreach(RangeByLevel spawnPercentByLevel in ammoSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevelSO == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    private int GetHealthPercentToSpawn(int healthNum)
    {

        if (healthNum == 0) return 0;

        foreach (RangeByLevel spawnPercentByLevel in healthSpawnByLevelList)
        {
            if (spawnPercentByLevel.dungeonLevelSO == GameManager.Instance.GetCurrentDungeonLevel())
            {
                return Random.Range(spawnPercentByLevel.min, spawnPercentByLevel.max);
            }
        }

        return 0;
    }

    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNum)
    {
        if (weaponNum == 0) return null;

        RandomSpawnableObject<WeaponDetailsSO> randomWeapon = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);
        WeaponDetailsSO weaponDetailsSO = randomWeapon.GetItem();
        
        return weaponDetailsSO;
    }

    private bool RandomSpawnChest()
    {
        int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);
        
        foreach(RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
        {
            if (rangeByLevel.dungeonLevelSO == GameManager.Instance.GetCurrentDungeonLevel())
            {
                chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                break;
            }
        }

        int randomChange = Random.Range(1, 100 + 1);
        return randomChange <= chancePercent;
    }

    private void GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum)
    {
        ammoNum = 0;
        healthNum = 0;
        weaponNum = 0;

        int numberOfItemsToSpawn = Random.Range(numberOfItemsToSpawnMin, numberOfItemsToSpawnMax + 1);
        int choice = Random.Range(0, 3);

        if (numberOfItemsToSpawn == 1)
        {
            if (choice == 0) { weaponNum++; return; }
            if (choice == 1) { ammoNum++; return; }
            if (choice == 2) { healthNum++; return; }
            return;
        }
        else if (numberOfItemsToSpawn == 2)
        {
            if (choice == 0) { weaponNum++; ammoNum++; return; }
            if (choice == 1) { ammoNum++; healthNum++; return; }
            if (choice == 2) { weaponNum++; healthNum++; return; }
            return;
        }
        else if (numberOfItemsToSpawn == 3)
        {
            weaponNum++;
            healthNum++;
            ammoNum++;
            return;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(chestSpawnChanceMin), chestSpawnChanceMin,
            nameof(chestSpawnChanceMax), chestSpawnChanceMax, true);

        if (chestSpawnChanceByLevelList != null && chestSpawnChanceByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevelList), chestSpawnChanceByLevelList);

            foreach(RangeByLevel rangeByLevel in chestSpawnChanceByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevelSO), rangeByLevel.dungeonLevelSO);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                    nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(numberOfItemsToSpawnMin), numberOfItemsToSpawnMin,
            nameof(numberOfItemsToSpawnMax), numberOfItemsToSpawnMax, true);

        if (weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0)
        {
            foreach (SpawnableObjectsByLevel<WeaponDetailsSO> weaponDetailsByLevel in weaponSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(weaponDetailsByLevel.dungeonLevelSO), weaponDetailsByLevel.dungeonLevelSO);
                
                foreach (SpawnableObjectRatio<WeaponDetailsSO> weaponRatios in weaponDetailsByLevel.spawnableObjectRatioList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatios.dungeonObject), weaponRatios.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatios.ratio), weaponRatios.ratio, true);
                }
            }
        }

        if (healthSpawnByLevelList != null && healthSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthSpawnByLevelList), healthSpawnByLevelList);

            foreach(RangeByLevel rangeByLevel in healthSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevelSO), rangeByLevel.dungeonLevelSO);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                    nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }

        if (ammoSpawnByLevelList != null && ammoSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoSpawnByLevelList), ammoSpawnByLevelList);
            foreach(RangeByLevel rangeByLevel in ammoSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.dungeonLevelSO), rangeByLevel.dungeonLevelSO);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                    nameof(rangeByLevel.max), rangeByLevel.max, true);
            }
        }
    }
#endif
    #endregion
}
