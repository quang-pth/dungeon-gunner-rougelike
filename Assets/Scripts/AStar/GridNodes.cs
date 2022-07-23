using UnityEngine;

// This class holds all the nodes in the instantiated room
public class GridNodes
{
    private int width;
    private int height;
    private Node[,] gridNode;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPos, int yPos)
    {
        // Check if position is in range
        if (xPos < width && yPos < height)
        {
            try
            {
                return gridNode[xPos, yPos];
            }
            catch
            {
                Debug.Log("Index out of bounds");
                return gridNode[0, 0];
            }
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
}
