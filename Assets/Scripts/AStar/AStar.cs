using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPos, Vector3Int endGridPos)
    {
        // calc the grid relative position on the tilemap
        startGridPos -= (Vector3Int)room.templateLowerBounds;
        endGridPos -= (Vector3Int)room.templateLowerBounds;

        // This list contains not processed nodes and needed to be sorted
        List<Node> openNodeList = new List<Node>();
        // This set contains processed nodes
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        // Create grid nodes in the room
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPos.x, startGridPos.y);
        Node targetNode = gridNodes.GetGridNode(endGridPos.x, endGridPos.y);

        // Get the end path node by using the A* algorithm
        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, room.instantiatedRoom);

        if (endPathNode != null)
        {
            // Create the path by traversing upwards the endPathNode's parent
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    private static Stack<Vector3> CreatePathStack(Node currentNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0;

        Node parentNode = currentNode;
        // Traverse upwards the current node's parent to get the path stack
        while (parentNode != null)
        {
            Vector3Int nodeGridPos = new Vector3Int(parentNode.gridPosition.x + room.templateLowerBounds.x, parentNode.gridPosition.y + room.templateLowerBounds.y, 0);
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(nodeGridPos);

            worldPosition += cellMidPoint;
            movementPathStack.Push(worldPosition);

            parentNode = parentNode.parentNode;
        }

        return movementPathStack;
    }
    
    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            // Sort the list by the node's cost
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            // Finish if found the target node
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            // Add the already processed node to the closed hash set
            closedNodeHashSet.Add(currentNode);

            // Evaluate the current node's neighbor
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom);
        }

        // return null if not found any path
        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        if (currentNode == null) return;

        Vector2Int currentGridPos = currentNode.gridPosition;

        Node validNeighbourNode;

        // Loop through all 8 neighbours of the current node
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Not processed the current node
                if (i == 0 && j == 0) continue;

                validNeighbourNode = GetValidNodeNeighbour(currentGridPos.x + i, currentGridPos.y + j, gridNodes, closedNodeHashSet, instantiatedRoom);

                if (validNeighbourNode != null)
                {
                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];
                    // Calc new gcost for the neighbour node
                    int newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;
                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.GCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.GCost = newCostToNeighbour;
                        validNeighbourNode.HCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node startNode, Node endNode)
    {
        int xDistance = Mathf.Abs(startNode.gridPosition.x - endNode.gridPosition.x);
        int yDistance = Mathf.Abs(startNode.gridPosition.y - endNode.gridPosition.y);

        // diagnal move takes 14 cost
        // vertical and horizontal move: takes 10 cost
        if (xDistance > yDistance)
        {
            return 14 * yDistance + 10 * (xDistance - yDistance);
        }

        return 14 * xDistance + 10 * (yDistance - xDistance);
    }

    private static Node GetValidNodeNeighbour(int xPos, int yPos, GridNodes gridNodes, HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        int xBounds = instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x;
        int yBounds = instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y;
        bool xBoundIsValid = xPos >= 0 && xPos < xBounds;
        bool yBoundIsValid = yPos >= 0 && yPos < yBounds;

        if (!xBoundIsValid || !yBoundIsValid)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(xPos, yPos);

        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[xPos, yPos];

        if (closedNodeHashSet.Contains(neighbourNode) || movementPenaltyForGridSpace == 0)
        {
            return null;
        }

        return neighbourNode;
    }
}
