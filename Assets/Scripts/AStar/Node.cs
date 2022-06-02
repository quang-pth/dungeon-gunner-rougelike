using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int GCost = 0; // distance from the node to starting node
    public int HCost = 0; // distance from the node to ending node
    public Node parentNode;
    
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        parentNode = null;
    }

    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compareResult = FCost.CompareTo(nodeToCompare.FCost);

        if (compareResult == 0)
        {
            compareResult = HCost.CompareTo(nodeToCompare.HCost);
        }
        
        return compareResult;
    }

}
