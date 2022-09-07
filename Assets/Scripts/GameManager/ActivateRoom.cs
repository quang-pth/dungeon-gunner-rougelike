using System.Collections.Generic;
using UnityEngine;

public class ActivateRoom : MonoBehaviour
{
    #region Header MINIMAP CAMERA
    [Space(10)]
    [Header("MINIMAP CAMERA")]
    #endregion
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        if (GameManager.Instance.gameState == GameState.dungeonOverviewmap) return;

        HelperUtilities.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldLowerBounds, out Vector2Int miniMapCameraWorldUpperBounds, miniMapCamera);
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldLowerBounds, out Vector2Int mainCameraWorldUpperBounds, mainCamera);

        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            bool roomIsInBoundsX = room.upperBounds.x >= miniMapCameraWorldLowerBounds.x && room.lowerBounds.x <= miniMapCameraWorldUpperBounds.x;
            bool roomIsInBoundsY = room.upperBounds.y >= miniMapCameraWorldLowerBounds.y && room.lowerBounds.y <= miniMapCameraWorldUpperBounds.y;
            // Activate rooms within minimap camera visable range
            if (roomIsInBoundsX && roomIsInBoundsY)
            {
                room.instantiatedRoom.gameObject.SetActive(true);

                // Activate environment items if within the main camera fov
                roomIsInBoundsX = room.upperBounds.x >= mainCameraWorldLowerBounds.x && room.lowerBounds.x <= mainCameraWorldUpperBounds.x;
                roomIsInBoundsY = room.upperBounds.y >= mainCameraWorldLowerBounds.y && room.lowerBounds.y <= mainCameraWorldUpperBounds.y;
                if (roomIsInBoundsX && roomIsInBoundsY)
                {
                    room.instantiatedRoom.ActivateEnvironmentGameObjects();
                    if (room.instantiatedRoom.room.isLit)
                    {
                        room.instantiatedRoom.ActivateFlameLighting();
                    }
                }
                else
                {
                    room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                    room.instantiatedRoom.DeactivateFlameLighting();
                }
            }
            else
            {
                room.instantiatedRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
    #endregion
}
