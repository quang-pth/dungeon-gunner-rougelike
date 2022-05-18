using Cinemachine;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate with the child MiniMapPlayer gameobject")]
    #endregion
    [SerializeField] private GameObject miniMapPlayer;

    private Transform playerTransform;

    private void Start() {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        // Set the virtual camera target to player transform
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        // Get the player minimap icon
        SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMiniMapIcon();
        }
    }

    private void Update() {
        // Update the player minimap icon position based on the player position
        if (playerTransform != null && miniMapPlayer != null) {
            miniMapPlayer.transform.position = playerTransform.position;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(miniMapPlayer), miniMapPlayer);
    }
#endif
    #endregion
}
