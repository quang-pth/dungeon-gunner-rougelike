using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty;
    [HideInInspector] public int[,] aStarItemObstacles;
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();
    
    [HideInInspector] public int Width
    {
        get
        {
            return room.templateUpperBounds.x - room.templateLowerBounds.x;
        }
    } 
    [HideInInspector] public int Height
    {
        get
        {
            return room.templateUpperBounds.y - room.templateLowerBounds.y;
        }
    }

    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the environment child placeholder gameobject")]
    #endregion
    [SerializeField] private GameObject environmentGameObject;
    [SerializeField] private GameObject flameItem;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        roomColliderBounds = boxCollider2D.bounds;
    }

    private void Start()
    {
        UpdateMoveableObstacles();
    }

    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveItem moveItem in moveableItemsList)
        {
            Vector3Int colliderBoundMin = grid.WorldToCell(moveItem.boxCollider2D.bounds.min);
            Vector3Int colliderBoundMax = grid.WorldToCell(moveItem.boxCollider2D.bounds.max);
            
            for (int i = colliderBoundMin.x; i <= colliderBoundMax.x; i++)
            {
                for (int j = colliderBoundMin.y; j <= colliderBoundMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < Width + 1; i++)
    //    {
    //        for (int j = 0; j < Height + 1; j++)
    //        {
    //            if (aStarItemObstacles[i, j] == 0)
    //            {
    //                Vector3 worldCellPosition = grid.CellToWorld(new Vector3Int(i + room.templateLowerBounds.x, j + room.templateLowerBounds.y, 0));
    //                Gizmos.DrawWireCube(new Vector3(worldCellPosition.x + 0.5f, worldCellPosition.y + 0.5f, 0f), Vector3.one);
    //            }
    //        }
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision) {
        // Make the room appear if player collides with the room
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom()) {
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorways();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRoom();

        DisableCollisionTilemapRenderer();
    }

    private void CreateItemObstaclesArray()
    {
        aStarItemObstacles = new int[Width + 1, Height + 1];
    }

    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < Width + 1; x++)
        {
            for (int y = 0; y < Height + 1; y++)
            {
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    private void BlockOffUnusedDoorways()
    {
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected) continue;

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }
        }
    }

    public void DeactivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(false);
        }
    }

    public void ActivateEnvironmentGameObjects()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(true);
        }
    }

    public void ActivateFlameLighting()
    {
        if (flameItem != null)
        {
            flameItem.SetActive(true);
        }
    }

    public void DeactivateFlameLighting()
    {
        if (flameItem != null)
        {
            flameItem.SetActive(false);
        }
    }

    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
        }
    }

    // Block doorway horizontally for NS doorway
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // Copy the tile from top to bottom - left to right
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                Vector3Int originalTilePosition = new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0);

                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(originalTilePosition);

                // Copy the original tile to the new position
                Vector3Int positionToPlace = new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0);
                TileBase tileToPlace = tilemap.GetTile(originalTilePosition);
                tilemap.SetTile(positionToPlace, tileToPlace);

                // Set the new tile's rotation of tile copied
                tilemap.SetTransformMatrix(positionToPlace, transformMatrix);
            }
        }
    }


    // Block doorway horizontally for EW doorway
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // Copy the tile from left to right - top to bottom
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                Vector3Int originalTilePositon = new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0);

                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(originalTilePositon);

                Vector3Int positionToPlace = new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0);
                TileBase tileToPlace = tilemap.GetTile(originalTilePositon);
                tilemap.SetTile(positionToPlace, tileToPlace);

                tilemap.SetTransformMatrix(positionToPlace, transformMatrix);
            }
        }
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.CompareTag("groundTilemap"))
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("decoration1Tilemap"))
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("decoration2Tilemap"))
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("frontTilemap"))
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("collisionTilemap"))
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.CompareTag("minimapTilemap"))
            {
                minimapTilemap = tilemap;
            }
        }
    }
    
    private void AddObstaclesAndPreferredPaths()
    {
        aStarMovementPenalty = new int[Width + 1, Height + 1];

        for (int xPos = 0; xPos < Width + 1; xPos++)
        {
            for (int yPos = 0; yPos < Height + 1; yPos++)
            {
                aStarMovementPenalty[xPos, yPos] = Settings.defaultAStarMovementPenalty;

                Vector3Int tilePos = new Vector3Int(xPos + room.templateLowerBounds.x, yPos + room.templateLowerBounds.y, 0);
                TileBase tile = collisionTilemap.GetTile(tilePos);

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        // This position is an obstacle
                        aStarMovementPenalty[xPos, yPos] = 0;
                        break;
                    }
                }

                if (tile == GameResources.Instance.preferredEnemyPathTile)
                {
                    aStarMovementPenalty[xPos, yPos] = Settings.preferredAStarMovementPenalty;
                }
            }
        }
    }

    private void AddDoorsToRoom() 
    {
        // Not placing a door in a corridor
        if (room.roomNodeType.isCooridorEW || room.roomNodeType.isCooridorNS) return;

        // Instantiate each doorway
        foreach (Doorway doorway in room.doorwayList) {
            if (doorway.doorPrefab != null && doorway.isConnected) {
                float tileDistance = Settings.tileSizePixels / Settings.pixelPerUnit;

                GameObject door = Instantiate(doorway.doorPrefab, gameObject.transform);

                // Place the door with the correspond doorway's orientation
                if (doorway.orientation == Orientation.north) {
                    // Offset the door half of Unity unit to the left and one unit down
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south) {
                    // Offset the door half of Unity unit to the left
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east) {
                    // Offset the door one Unity unit to the left and 1.25 unit down
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0f);
                }
                else if (doorway.orientation == Orientation.west) {
                    // Offset the door 1.25 Unity unit down
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0f);
                }

                Door doorComponent = door.GetComponent<Door>();
                // Lock the boss room door
                if (room.roomNodeType.isBossRoom) {
                    doorComponent.isBossRoomDoor = true;
                    doorComponent.LockDoor();
                }
            }
        }

    }
    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    public void DisableRoomCollider()
    {
        boxCollider2D.enabled = false;
    }
    
    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }

    public void LockDoors()
    {
        Door[] doorways = GetComponentsInChildren<Door>();

        foreach (Door door in doorways)
        {
            door.LockDoor();
        }

        DisableRoomCollider();
    }

    public void UnclockDoors(float doorUnclockDelay)
    {
        StartCoroutine(UnclockDoorsRoutine(doorUnclockDelay));
    }

    private IEnumerator UnclockDoorsRoutine(float doorUnclockDelay)
    {
        if (doorUnclockDelay > 0)
        {
            yield return new WaitForSeconds(doorUnclockDelay);
        }

        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach(Door door in doorArray)
        {
            door.UnclockDoor();
        }

        EnableRoomCollider();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }
#endif
    #endregion
}
