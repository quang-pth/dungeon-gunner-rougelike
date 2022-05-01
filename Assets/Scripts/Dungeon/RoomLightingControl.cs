using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiatedRoom instantiatedRoom;

    private void Awake() {
        instantiatedRoom = GetComponent<InstantiatedRoom>();
    }

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs) {
        if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit) {
            FadeInRoomLighting();

            FadeInDoors();

            instantiatedRoom.room.isLit = true;
        }
    }

    private void FadeInRoomLighting() {
        StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
    }

    private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom) {
        Material material = new Material(GameResources.Instance.variableLitShader);

        // Set material for tilemaps
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        // Fade in the room
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime) {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        // Lit all the instatiated room tilemap
        instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;        
    }

    private void FadeInDoors() {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray) {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();
            doorLightingControl.FadeInDoor(door);
        }
    }
}
