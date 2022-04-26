using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region Tooltip
    [Tooltip("Populate the CursorTarget gameobject")]
    #endregion
    [SerializeField] private Transform cursorTarget;

    private void Awake() {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start() {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup() {
        Transform targetTransform = GameManager.Instance.GetPlayer().transform;
        if (!targetTransform) return;
        // Create cinemachine target group for cinemachine camera to follow
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 2.5f,
            target = targetTransform
        };

        // Cinemachine to follow the game screen cursor
        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] {
            cinemachineGroupTarget_player, cinemachineGroupTarget_cursor
        };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update() {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
