using UnityEngine;
using System;

public static class Settings
{
    #region UNITS
    public const float pixelPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion
    
    #region DUNGEON BULDING SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public const float fadeInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const float doorUnclockDelay = 1f;
    #endregion

    #region  ANIMATOR PARAMETERS
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static float baseSpeedForPlayerAnimations = 8f;
    public static float baseSpeedForEnemyAnimations = 3f;
    public static int open = Animator.StringToHash("open");
    public static int destroy = Animator.StringToHash("destroy");
    public static String destroyedState = "Destroyed";
    #endregion

    #region GAMEOBJECT TAGS
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f; // if the target distance is less this variable then the aim angle
                                                   // will be used otherwise the weapon aim angle will be used
    #endregion

    #region ASTAR PATHFINDING PARAMETERS
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathFindingOver = 60;
    public const float playerMoveDistanceToRebuildPath= 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion
}
