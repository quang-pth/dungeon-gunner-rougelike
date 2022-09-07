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
    [HideInInspector] public int updateFrameNumber = 1;
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

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

        if (!chasePlayer)
        {
            return;
        }

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber)
        {
            return;
        }

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

    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
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

        int cellPenalty;
        // Pevent index out of range in some edge cases
        try
        {
            int penalty = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPos.x, adjustedPlayerCellPos.y];
            int obstacles = currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPos.x, adjustedPlayerCellPos.y];
            cellPenalty = Mathf.Min(penalty, obstacles);
        }
        catch
        {
            return new Vector3Int(0, 0, 0);
        }

        // current player pos is not an obstacle
        if (cellPenalty != 0)
        {
            return playerCellPos;
        }
        // Get non-obstacle grid around the current player position
        else
        {
            surroundingPositionList.Clear();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }
            for (int l = 0; l < 8; l++)
            {
                int index = Random.Range(0, surroundingPositionList.Count);
                try
                {
                    int penalty = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPos.x
                        + surroundingPositionList[index].x, adjustedPlayerCellPos.y + surroundingPositionList[index].y];
                    int itemObstacle = currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPos.x
                        + surroundingPositionList[index].x, adjustedPlayerCellPos.y + surroundingPositionList[index].y];
                    int obstacle = Mathf.Min(penalty, itemObstacle);
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPos.x + surroundingPositionList[index].x,
                            playerCellPos.y + surroundingPositionList[index].y, 0);
                    }
                }
                catch
                {

                }
                surroundingPositionList.RemoveAt(index);
            }

            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
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
