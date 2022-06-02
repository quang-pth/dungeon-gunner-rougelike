using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startGridPos;
    private Vector3Int endGridPos;
    private TileBase startPathTile;
    private TileBase endPathTile;
    private Grid grid;

    private Vector3Int initValue = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    private Stack<Vector3> pathStack;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        startPathTile = GameResources.Instance.preferredEnemyPathTile;
        endPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiatedRoom = roomChangedEventArgs.room.instantiatedRoom;
        frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        grid = instantiatedRoom.GetComponentInChildren<Grid>();
        startGridPos = initValue;
        endGridPos = initValue;

        SetUpPathTilemap();
    }

    private void SetUpPathTilemap()
    {
        Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }
        else
        {
            pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    private void Update()
    {
        if (instantiatedRoom == null || startPathTile == null || endPathTile == null || grid == null || pathTilemap == null) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }        
    }

    private void SetStartPosition()
    {
        if (startGridPos == initValue)
        {
            startGridPos = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBound(startGridPos))
            {
                startGridPos = initValue;
                return;
            }

            pathTilemap.SetTile(startGridPos, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPos, null);
            startGridPos = initValue;
        }
    }

    private void SetEndPosition()
    {
        if (endGridPos == initValue)
        {
            endGridPos = grid.WorldToCell(HelperUtilities.GetMouseWorldPosition());

            if (!IsPositionWithinBound(endGridPos))
            {
                endGridPos = initValue;
                return;
            }

            pathTilemap.SetTile(endGridPos, endPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPos, null);
            endGridPos = initValue;
        }
    }

    private bool IsPositionWithinBound(Vector3Int startGridPos)
    {
        bool isInXBound = startGridPos.x >= instantiatedRoom.room.templateLowerBounds.x && startGridPos.x <= instantiatedRoom.room.templateUpperBounds.x;
        bool isInYBound = startGridPos.y >= instantiatedRoom.room.templateLowerBounds.y && startGridPos.x <= instantiatedRoom.room.templateUpperBounds.y;

        return isInXBound && isInYBound;
    }

    private void DisplayPath()
    {
        if (startGridPos == null || endGridPos == null)
        {
            return;
        }

        pathStack = AStar.BuildPath(instantiatedRoom.room, startGridPos, endGridPos);
        if (pathStack == null) return;

        foreach(Vector3 gridWorldPos in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(gridWorldPos), startPathTile);
        }
    }

    private void ClearPath()
    {
        if (pathStack == null) return;

        foreach(Vector3 gridWorldPos in pathStack)
        {
            pathTilemap.SetTile(grid.WorldToCell(gridWorldPos), null);
        }
        pathStack = null;

        startGridPos = initValue;
        endGridPos = initValue;
    }
}
