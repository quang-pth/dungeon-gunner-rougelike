using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Header BASIC LEVEL DETAILS
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    #endregion Header BASIC LEVEL DETAILS

    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion Tooltip
    public string levelName;

    #region Header ROOM TEMPLATES FOR LEVEL
    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    #endregion Header ROOM TEMPLATES FOR LEVEL
    #region Tooltip
    [Tooltip("Populate the room templats that you want to be part of the level. You need to ensure that room templates are include" +
        " for all room node types that are specified in the room node graphs for the level.")]
    #endregion Tooltip
    public List<RoomTemplateSO> roomTemplateList;

    #region Header ROOM NODE GRAPHS FOR LEVEL
    [Space(10)]
    [Header("ROOM NODE GRAPHS FOR LEVEL")]
    #endregion Header ROOM NODE GRAPHS FOR LEVEL
    #region Tooltip
    [Tooltip("Populate this list with the room node graphs which should be selected randomly for the level.")]
    #endregion Tooltip
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        // Check room templates are specified for all node types.

        // First check all corridors and entrance types has been specified
        bool isNSCorridor = false;
        bool isEWCorridor = false;
        bool isEntrance = false;
        foreach(RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null) return;

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
            
            if (roomTemplateSO.roomNodeType.isCooridorNS)
            {
                isNSCorridor = true;
            }
            
            if (roomTemplateSO.roomNodeType.isCooridorEW)
            {
                isEWCorridor = true;
            }
        }

        if (!isEntrance)
        {
            Debug.Log("In " + this.name.ToString() + " : No Entrance Corridor room type specified");
        }
        
        if (!isNSCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No N/S Corridor room type specified");
        }
        
        if (!isEWCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " : No E/W Corridor room type specified");
        }

        // Check room template has been specified for each room node type
        foreach(RoomNodeGraphSO roomNodeGraphSO in roomNodeGraphList)
        {
            if (roomNodeGraphSO == null) return;
            
            // Loop through each room node 
            foreach(RoomNodeSO roomNodeSO in roomNodeGraphSO.roomNodeList)
            {
                if (roomNodeSO == null) continue;

                RoomNodeTypeSO roomNodeType = roomNodeSO.roomNodeType;
                if (roomNodeType.isEntrance || roomNodeType.isCorridor || roomNodeType.isCooridorEW
                    || roomNodeType.isCooridorNS || roomNodeType.isNone) continue;

                bool isRoomNodeTypeFound = false;
                // Loop through each room template
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null) continue;

                    if (roomTemplateSO.roomNodeType == roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }
                
                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + ": No room template " + roomNodeType.name.ToString() + " found for node graph " +
                        roomNodeGraphSO.name.ToString());
                }
            }
        }

    }

#endif
    #endregion Validation
} 
