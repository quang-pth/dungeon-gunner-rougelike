using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

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
            radius = 1f,
            target = targetTransform
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] {
            cinemachineGroupTarget_player
        };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }
}
