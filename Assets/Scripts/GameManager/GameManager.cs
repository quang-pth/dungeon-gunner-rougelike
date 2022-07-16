using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager>
{
    #region Header DUNGEON LEVELS
    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS
    #region Tooltip
    [Tooltip("Populate with the dungeon level scriptable objects")]
    #endregion Tooltip
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip
    [Tooltip("Populate with the dungeon level for testing, first level = 0")]
    #endregion Tooltip
    [SerializeField] private int currentDungeonLevelListIdx = 0;
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore = 0;
    private long scoreMultiplier = 1;
    [SerializeField] private int increaseMultiplierFactor = 1;
    [SerializeField] private int decreaseMultiplierFactor = 3;
    private Room bossRoom = null;

    protected override void Awake()
    {
        base.Awake();

        // Set the player details - saved in the current game resources
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    private void OnEnable() {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable() {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs roomEnemiesDefeatedEventArgs)
    {
        RoomEnemiesDefeated();
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;
        // Get the boss room and check if a dungeon level is ready for the boss stage or not
        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;
            if (room.instantiatedRoom.room.roomNodeType.isBossRoom)
            {
                bossRoom = room;
                continue;
            }
            if (!room.instantiatedRoom.room.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }
        // End game if all dungeon level is completed
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.isClearedOfEnemies))
        {
            if (currentDungeonLevelListIdx < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;
            StartCoroutine(BossStage());
        }
    }

    private IEnumerator BossStage()
    {
        bossRoom.instantiatedRoom.gameObject.SetActive(true);
        bossRoom.instantiatedRoom.UnclockDoors(0f);
        yield return new WaitForSeconds(2f);
        Debug.Log("Find and destroy the BOSS please");
    }

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEvetArgs destroyedEvetArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier += increaseMultiplierFactor;
        }
        else
        {
            scoreMultiplier -= decreaseMultiplierFactor;
        }

        scoreMultiplier = (long) Mathf.Clamp(scoreMultiplier, 1, 30);
        //  Update the multiplier score on the UI
        StaticEventHandler.CallOnScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        gameScore += pointsScoredArgs.points * scoreMultiplier;
        // Update the score on the UI
        StaticEventHandler.CallOnScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs) {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    // Start is called before the first frame update
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
        gameScore = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        // Press P to change the dungeon layout
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     gameState = GameState.gameStarted;
        // }
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIdx);
                gameState = GameState.playingLevel;
                // Trigger just in case (dungeon level has only 1 entrace and a boss room)
                RoomEnemiesDefeated();
                break;
            case GameState.levelCompleted:
                StartCoroutine(LevelCompleted());
                break;
            case GameState.gameWon:
                // Only trigger once (just in case you win a dungeon level and come to the next dungeon level which has only one entrace)
                if (previousGameState != GameState.gameWon)
                {
                    StartCoroutine(GameWon());
                }
                break;
            case GameState.gameLost:
                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); // Prevent game won messages if you cleared the dungeon just as you get killed
                    StartCoroutine(GameLost());
                }
                break;
            case GameState.restartGame:
                RestartGame();
                break;
        }
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;
        
        Debug.Log("Level Completed - Game will move to the next dungeon in 10s");
        yield return new WaitForSeconds(10f);
        currentDungeonLevelListIdx++;
        PlayDungeonLevel(currentDungeonLevelListIdx);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        Debug.Log("All dungeon levels are completed - Game will restart in 10s");
        yield return new WaitForSeconds(10f);
        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;
        Debug.Log("You died - Game will restart in 10s");
        yield return new WaitForSeconds(10f);
        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        currentDungeonLevelListIdx = 0;
        SceneManager.LoadScene("MainGameScene");
    }

    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void PlayDungeonLevel(int dungeonLevelListIdx)
    {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIdx]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.Log("Couldn't build dungeon from specified rooms and node graphs");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set the player roughly mid-room
        float xPosition = (currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2;
        float yPosition = (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2;
        player.gameObject.transform.position = new Vector3(xPosition, yPosition, 0);

        // Get nearest spawn poisition in room to the player - convert grid coordinate to game world coordinates
        player.gameObject.transform.position = HelperUtilities.GetPositionNearestToPlayer(player.gameObject.transform.position);

        RoomEnemiesDefeated();
    }

    public Player GetPlayer() {
        return player;
    }

    public Sprite GetPlayerMiniMapIcon() {
        return playerDetails.playerMinimapIcon;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public DungeonLevelSO GetCurrentDungeonLevel()
    { 
        return dungeonLevelList[currentDungeonLevelListIdx];
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion Validation
}
