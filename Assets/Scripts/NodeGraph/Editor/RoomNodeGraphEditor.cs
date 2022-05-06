using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private static RoomNodeTypeListSO roomNodeTypeList;

    // Dragging the graph
    private Vector2 graphOffset;
    private Vector2 graphDrag;

    // Node layout values
    private const float nodeWidth = 160f;
    private const float nodeHeigth = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting Line values
    private const float connectingLineWidth = 3.0f;
    private const float connectingLineArrowSize = 6.0f;

    // Draw the gird lines
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow() {
        // Open Node Editor Window and Set its name to Room Node Graph Editor
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Set Room Node Style
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        // Define selected room node
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        // Load room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        // Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void InspectorSelectionChanged()
    {
        // Get current item is currently selected in the inspector
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        // Open it if it's a room node graph
        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;

            GUI.changed = true;
        }
    }


    // Open a room node graph editor window if a room node graph scritable object asset is double clicked in the inspector
    [OnOpenAsset(0)] // namespace from UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }

    // Draw Editor GUI
    private void OnGUI() 
    {
        if (currentRoomNodeGraph != null)
        {
            // Draw small and large grid lines
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.2f, Color.gray);

            // Draw drag line
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw room node connection lines
            DrawRoomConnections();

            // Draw Room Nodes;
            DrawRoomNodes();
        }

        if (GUI.changed) Repaint();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt(position.width / gridSize + 1);
        int horizontalLineCount = Mathf.CeilToInt(position.height/ gridSize + 1);

        // Set grid lines drawing color
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;
        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        // Draw vertical lines
        for (int i = 0; i < verticalLineCount; i++)
        {
            Vector3 startPosition = new Vector3(gridSize * i, -gridSize, 0) + gridOffset;
            Vector3 endPosition = new Vector3(gridSize * i, position.height + gridSize, 0) + gridOffset;
            Handles.DrawLine(startPosition, endPosition);
        }
        // Draw horizontal lines
        for (int j = 0; j < horizontalLineCount; j++)
        {
            Vector3 startPosition = new Vector3(-gridSize, gridSize * j, 0) + gridOffset;
            Vector3 endPosition = new Vector3(position.width + gridSize, gridSize * j, 0) + gridOffset;
            Handles.DrawLine(startPosition, endPosition);
        }

        // Reset the 
        Handles.color = Color.white;
    }

    private void DrawRoomConnections()
    {
        // For each room node in the graph draw connection to its children
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // If room node has at least one children
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                // Loop through room node's children ID
                foreach(string childrenRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    // Draw connection if the child ID is in the room node graph's node dictionary
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childrenRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childrenRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childrenRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childrenRoomNode.rect.center;

        // Mid position
        Vector2 midPosition = (endPosition + startPosition) / 2.0f;
        // Direction to draw
        Vector2 direction = endPosition - startPosition;
        // Calc arrow tails
        Vector2 arrowTailPoint1 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        // Arrow head point 
        Vector2 arrowHead = midPosition + direction.normalized * connectingLineArrowSize;
        // Draw Arrow from head point to tail
        Handles.DrawBezier(arrowHead, arrowTailPoint1, arrowHead, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHead, arrowTailPoint2, arrowHead, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // Draw parent and children room node connection 
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);
        
        GUI.changed = true;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            // Draw line from start to end position
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        graphDrag = Vector2.zero;

        if (currentRoomNode == null || currentRoomNode.isLeftClickedDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        // Room Node Graph events for mouse isn't being hovered or we're drawing a connetion line
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcesRoomNodeGraphEvents(currentEvent);
        }
        // Room Node events
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    // Check if mouse is over a particular room node - return room node else null
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ProcesRoomNodeGraphEvents(Event currentEvent)
    {
        switch(currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag: // draw room node connection line event
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // Set room node as children of the roomNodeToDrawLineFrom
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    // Set roomNodeToDrawLineFrom as parent
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;
        // Drag every room nodes on the graph according to the dragDelta
        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++) 
        {
            RoomNodeSO currentRoomNode = currentRoomNodeGraph.roomNodeList[i];
            currentRoomNode.DragNode(graphDrag);
        }
        GUI.changed = true;
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    // Process Mouse Down events on the room node graph (not over a node)
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Process right click mouse down on room node graph
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeSelection = new Queue<RoomNodeSO>();

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            // Delete selected room nodes (not an entrance node)
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                // Add the node to queue
                roomNodeSelection.Enqueue(roomNode);

                // Delete its links to child room nodes
                foreach(string childId in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childId);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                // Delete its links to parent room nodes
                foreach(string parentId in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentId);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        // Delete selected room node
        while (roomNodeSelection.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeSelection.Dequeue();
            // Remove room node from the room node graph
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            // Remove room node from the room node list
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            // Remove room node from asset database
            DestroyImmediate(roomNodeToDelete, true);
            // Save asset database
            AssetDatabase.SaveAssets();
        }
    }

    // Delete the links between room nodes
    private void DeleteSelectedRoomNodeLinks()
    {
        // Iterate through all room nodes
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);
                    // Delete links between two selected room nodes
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        // Delete parentID from child room node
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        // Delete childID from parent room node
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void SelectAllRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        // Add entrance room node first if graph is empty
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // Create room node scriptabnle object asset
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // Add room node to room node graph
        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        // Set room node values
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeigth)), currentRoomNodeGraph, roomNodeType);

        // Add room node to room node graph SO Asset Database
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh the room node graph dictionary
        currentRoomNodeGraph.OnValidate();
    }

    // Draw room nodes in the room node graph window
    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            } 
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }
}
