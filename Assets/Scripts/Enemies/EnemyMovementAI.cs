using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetailsSO;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerRefPos;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        moveSpeed = movementDetailsSO.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        playerRefPos = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;
        Vector3 currentPlayerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        if (!chasePlayer && Vector3.Distance(transform.position, currentPlayerPosition) < enemy.enemyDetailsSO.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer) return;

        // Rebuild path to player if cooldown is reached or player moved a decent amount of distance
        if (currentEnemyPathRebuildCooldown <= 0f || Vector3.Distance(playerRefPos, currentPlayerPosition) >= Settings.playerMoveDistanceToRebuildPath)
        {
            // Reset path rebuild cooldown
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;
            // Reset ref to player position
            playerRefPos = currentPlayerPosition;

            // Create new path from enemy to player
            CreatePath();

            // Move enemy if path found
            if (movementSteps != null)
            {
                // Clear old path if already exist
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // Create new path for the enemy
                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }

        }
    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();
        Grid grid = currentRoom.instantiatedRoom.grid;
        Vector3Int playerGridPos = GetNearestNonObstaclePlayerPosition(currentRoom);
        Vector3Int enemyGridPos = grid.WorldToCell(transform.position);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPos, playerGridPos);

        if (movementSteps != null)
        {
            // pop the current enemy position
            movementSteps.Pop();
        }
        else
        {
            // Trigger enemy idle event if there's no path found
            enemy.idleEvent.CallIdleEvent();
        }
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPos = GameManager.Instance.GetPlayer().GetPlayerPosition();
        Vector3Int playerCellPos = currentRoom.instantiatedRoom.grid.WorldToCell(playerPos);
        Vector2Int adjustedPlayerCellPos = new Vector2Int(playerCellPos.x - currentRoom.instantiatedRoom.room.templateLowerBounds.x,
            playerCellPos.y - currentRoom.instantiatedRoom.room.templateLowerBounds.y);

        int cellPenalty = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPos.x, adjustedPlayerCellPos.y];

        // current player pos is not an obstacle
        if (cellPenalty != 0)
        {
            return playerCellPos;
        }
        // Get non-obstacle grid around the current player position
        else
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0) {
                        continue;
                    }

                    try
                    {
                        cellPenalty = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPos.x + i, adjustedPlayerCellPos.y + j];
                        if (cellPenalty != 0)
                        {
                            return new Vector3Int(playerCellPos.x + i, playerCellPos.y + j, 0);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return playerCellPos;
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPos = movementSteps.Pop();
            const float distanceToStop = 0.2f;

            // Move enemy to the next step position if not very close
            while (Vector3.Distance(nextPos, transform.position) > distanceToStop)
            {
                Vector3 moveDirection = (nextPos - transform.position).normalized;
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPos, transform.position, moveSpeed, moveDirection);
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetailsSO), movementDetailsSO);
    }
#endif
    #endregion
}
