using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region  Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion
    #region Tooltip
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculate a random value between the minimum and maximum")]
    #endregion
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The GetMoveSpeed method calculate a random value between the minimum and maximum")]
    #endregion
    public float maxMoveSpeed = 8f;

    public float GetMoveSpeed() {
        if (minMoveSpeed == maxMoveSpeed) return minMoveSpeed;

        return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
    }

#endif
    #endregion
}
