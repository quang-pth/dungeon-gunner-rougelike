using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickedDragging = false;
    [HideInInspector] public bool isSelected = false;
    // Initialise a room node
    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        // Load all room node types
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);

        // Detect Popup Selection Changes
        EditorGUI.BeginChangeCheck();

        // Lock a room node if it's entrance or the room node has parent
        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            // Display a label can't be changed
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            // Display a popup using RoomNodeType name values
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            // Room Node Type Selection Options
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];
        }

        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    // Get room node type that allowed to display in the room node graph editor
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickedDragging = true;

        // Move the room node by the mouse relative displacement
        DragNode(currentEvent.delta);

        GUI.changed = true;
    }

    private void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this); // save room node position when dragging
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        // Room node will no longer be dragged once the mouse is left clicked up
        if (isLeftClickedDragging)
        {
            isLeftClickedDragging = false;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Left Click on Room Node to drag it
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        // Right Click on Room Node to draw connection lines
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawLineFrom(this, currentEvent.mousePosition);
    }

    private void ProcessLeftClickDownEvent()
    {
        // Bind selected room node with the room node SO in project window
        Selection.activeObject = this;

        // Toglle isSelected variable
        isSelected = !isSelected;
    }

    // Add child room node id to the node - return true if the node has been added, otherwise false
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        return false;
    }

    private bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        foreach(RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            // Check if boss room node has alreay connected in the room node graph
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }
        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childID);
        // If the child has the type of boss room and boss room is already connected
        if (childRoomNode.roomNodeType.isBossRoom && isConnectedBossNodeAlready) return false;
        // If the child has type of None
        if (childRoomNode.roomNodeType.isNone) return false;
        // If the child already in the room node list
        if (childRoomNodeIDList.Contains(childID)) return false;
        // If the child is the same with the room node
        if (id == childID) return false;
        // If the childID is in the parentID list
        if (parentRoomNodeIDList.Contains(childID)) return false;
        // If the child node already has parentID 
        if (childRoomNode.parentRoomNodeIDList.Count > 0) return false;
        // If the child and the room node are corridors
        if (childRoomNode.roomNodeType.isCorridor && roomNodeType.isCorridor) return false;
        // If the child and the room node are not corridors
        if (!childRoomNode.roomNodeType.isCorridor && !roomNodeType.isCorridor) return false;
        // IF adding a corridor and this node has < permitted child room node
        if (childRoomNode.roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors) return false;
        // If the child node is the entrance
        if (childRoomNode.roomNodeType.isEntrance) return false;
        // If adding a room to a corridor check that this corridor node doesn't already have a room
        if (!childRoomNode.roomNodeType.isCorridor && childRoomNodeIDList.Count > 0) return false;

        return true;
    }

    // Add parent room node id to the node - return true if the node has been added, otherwise false
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

#endif
    #endregion Editor Code

}
