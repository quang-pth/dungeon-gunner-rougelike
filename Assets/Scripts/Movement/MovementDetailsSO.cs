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

    // For player
    #region Tooltip
    [Tooltip("If there is a roll movement, this is the roll speed")]
    #endregion
    public float rollSpeed;
    
    #region Tooltip
    [Tooltip("If there is a roll movement, this is the roll distance")]
    #endregion
    public float rollDistance;

    #region Tooltip
    [Tooltip("If there is a roll movement, this is the roll cool down time in seconds between roll actions")]
    #endregion
    public float rollCoolDownTime;

    public float GetMoveSpeed() {
        if (minMoveSpeed == maxMoveSpeed) return minMoveSpeed;

        return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollSpeed != 0 || rollDistance != 0 | rollCoolDownTime != 0) {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCoolDownTime), rollCoolDownTime, false);
        }
    }

#endif
    #endregion
}
