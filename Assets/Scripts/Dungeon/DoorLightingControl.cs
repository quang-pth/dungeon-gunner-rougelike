using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake() {
        // Get the Door component from the DoorNS or DoorEW gameobject
        door = GetComponentInParent<Door>();
    }

    public void FadeInDoor(Door door) {
        Material material = new Material(GameResources.Instance.variableLitShader);

        // Lit the room
        if (!isLit) {
            // Get all the door sprites. For the DoorNS case - 1 sprite, for the DoorEW case - 2 sprites
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray) {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }

            isLit = true;
        }
    }

    public IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material) {
        spriteRenderer.material = material;
        
        for (float i = 0.05f; i <= 1f; i += (Time.deltaTime / Settings.fadeInTime)) {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        FadeInDoor(door);
    }
}
