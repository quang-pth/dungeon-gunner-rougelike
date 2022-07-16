using System.Collections.Generic;
using UnityEngine;

public class ActivateRoom : MonoBehaviour
{
    #region Header MINIMAP CAMERA
    [Space(10)]
    [Header("MINIMAP CAMERA")]
    #endregion
    [SerializeField] private Camera miniMapCamera;

    private void Start()
    {
        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldLowerBounds, out Vector2Int miniMapCameraWorldUpperBounds, miniMapCamera);
        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            bool roomIsInBoundsX = room.upperBounds.x >= miniMapCameraWorldLowerBounds.x && room.lowerBounds.x <= miniMapCameraWorldUpperBounds.x;
            bool roomIsInBoundsY = room.upperBounds.y >= miniMapCameraWorldLowerBounds.y && room.lowerBounds.y <= miniMapCameraWorldUpperBounds.y;
            // Activate rooms within minimap camera visable range
            if (roomIsInBoundsX && roomIsInBoundsY)
            {
                room.instantiatedRoom.gameObject.SetActive(true);
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }
}
