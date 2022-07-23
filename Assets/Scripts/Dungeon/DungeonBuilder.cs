using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehavior<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable() {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable() {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts <= Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonBuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;
            while (!dungeonBuildSuccessful && dungeonBuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                ClearDungeon();

                dungeonBuildAttemptsForNodeGraph++;
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                InstantiateRoomObjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            } else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();
        RoomNodeSO entranceRoomNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceRoomNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceRoomNode);
        } else
        {
            Debug.Log("Entrance node not found!");
            return false;
        }

        // Start with no room overlaps
        bool noRoomOverlaps = true;

        noRoomOverlaps = ProcessRoomInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        return openRoomNodeQueue.Count == 0 && noRoomOverlaps;
    }
    private bool ProcessRoomInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Add the child room node of the current proccessing room node to the queue
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                // mark this room as placed
                room.isPositioned = true;

                // add entrance to the room dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                Room parentRoomNode = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoomNode);
            }
        }

        return noRoomOverlaps;
    }

    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoomNode)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            // Select available doorways of parent
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoomNode.doorwayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];
            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentParent(roomNode, doorwayParent);
            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            // Place the room - return true if the room doesn't overlaps
            if (PlaceTheRoom(parentRoomNode, doorwayParent, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true; // no room overlaps
    }

    private RoomTemplateSO GetRandomTemplateForRoomConsistentParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCooridorNS));
                    break;
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCooridorEW));
                    break;
                case Orientation.none:
                    break;
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    private bool PlaceTheRoom(Room parentRoomNode, Doorway doorwayParent, Room childRoom)
    {
        // Get the child room node Doorway corresponding to the parent doorway
        Doorway childDoorway = GetOppositeDoorway(doorwayParent, childRoom.doorwayList);
        if (childDoorway == null) return false;
        // Parent doorway position's relative to the parent room's lowerbound in the template
        Vector2Int parentDoorwayRelativePosition = doorwayParent.position - parentRoomNode.templateLowerBounds;
        // Parent Doorway position in the game world coordinates
        Vector2Int parentDoorwayPosition = parentRoomNode.lowerBounds + parentDoorwayRelativePosition;

        Vector2Int adjustment = Vector2Int.zero;
        switch (childDoorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }

        // Calc child doorway position in the game world coordinates
        childRoom.lowerBounds = parentDoorwayPosition + adjustment - (childDoorway.position - childRoom.templateLowerBounds);
        childRoom.upperBounds = childRoom.lowerBounds + (childRoom.templateUpperBounds - childRoom.templateLowerBounds);

        Room overlappingRoom = CheckForRoomOverlap(childRoom);

        if (overlappingRoom == null)
        {
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            childDoorway.isConnected = true;
            childDoorway.isUnavailable = true;

            return true;
        }
        else
        {
            doorwayParent.isUnavailable = true;

            return false;
        }
    }

    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (doorwayParent.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            if (doorwayParent.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
            if (doorwayParent.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            if (doorwayParent.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    // Check is room is overlapping - return room if not else null
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue;
            }

            if (IsOverLapping(room, roomToTest))
            {
                return room;
            }
        }

        return null;
    }

    private bool IsOverLapping(Room room, Room roomToTest)
    {
        bool isOverLappingX = IsOverLappingInterval(room.lowerBounds.x, room.upperBounds.x, roomToTest.lowerBounds.x, roomToTest.upperBounds.x);
        bool isOverLappingY = IsOverLappingInterval(room.lowerBounds.y, room.upperBounds.y, roomToTest.lowerBounds.y, roomToTest.upperBounds.y);

        return isOverLappingX && isOverLappingY;
    }

    private bool IsOverLappingInterval(int iMin1, int iMax1, int iMin2, int iMax2)
    {
        return Mathf.Max(iMin1, iMin2) <= Mathf.Min(iMax1, iMax2);
    }
    // Get a random room template matching the given room node type
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
        {
            return null;
        }

        // return a random room template
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> doorwayList)
    {
        foreach (Doorway doorway in doorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        // Set parent ID for the room - entrance node
        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // Set the entrance
            GameManager.Instance.SetCurrentRoom(room);
        } else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearedOfEnemies = true;
        }

        return room;
    }

    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        } else
        {
            Debug.Log("No room node graphs in list");
            return null;
        }
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> doorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in doorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;


            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    private List<string> CopyStringList(List<string> childRoomNodeIDList)
    {
        List<string> newString = new List<string>();

        foreach (string childId in childRoomNodeIDList)
        {
            newString.Add(childId);
        }

        return newString;
    }

    private void InstantiateRoomObjects()
    {
        foreach (KeyValuePair<string, Room> keyValuePair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponent<InstantiatedRoom>();

            instantiatedRoom.room = room;

            instantiatedRoom.Initialise(roomGameObject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }

        return null;
    }

    private Room GetRoomByID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }

        return null;
    }

    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyValue in dungeonBuilderRoomDictionary)
            {
                Room room = keyValue.Value;
                // Delete instantiated room
                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }
        }

        // Delete the dungeon 
        dungeonBuilderRoomDictionary.Clear();
    }
}
