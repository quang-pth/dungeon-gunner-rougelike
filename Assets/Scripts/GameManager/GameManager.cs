using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager>
{
    #region Header GAME OBJECTS REFERENCES
    [Space(10)]
    [Header("GAME OBJECTS REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with PauseMenuUI game object in hierarchy")]
    #endregion
    [SerializeField] private GameObject pauseMenu;
    #region Tooltip
    [Tooltip("Populate with the MessageText TMP component in the FadeScreenUI")]
    #endregion
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    #region Tooltip
    [Tooltip("Populate with the FadeImage Canvas Group component in the FadeScreenUI")]
    #endregion
    [SerializeField] private CanvasGroup canvasGroup;

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
    private bool isFading = false;

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

    private void DisplayDungeonOverviewMap()
    {
        if (isFading) return;

        DungeonMap.Instance.DisplayDungeonOverViewMap();
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
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        yield return StartCoroutine(DisplayTextMessage("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "| " +
            "YOU'VE SURVIVED....SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK BABE!", Color.white, 5f));
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
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
        scoreMultiplier = 1;
        StartCoroutine(Fade(1f, 0f, 0f, Color.black));
    }

    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        // Start fading the message text in and out
        float time = 0;
        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }

        isFading = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();
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
            case GameState.playingLevel:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    DisplayDungeonOverviewMap();
                }
                break;
            case GameState.engagingEnemies:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
            case GameState.dungeonOverviewmap:
                if (Input.GetKeyDown(KeyCode.M))
                {
                    DungeonMap.Instance.ClearDungeonOverViewMap();
                }
                break;
            case GameState.bossStage:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                if (Input.GetKeyDown(KeyCode.M))
                {
                    DisplayDungeonOverviewMap();
                }
                break;
            case GameState.engagingBoss:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
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
            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;
        }
    }

    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            gameState = previousGameState;
            previousGameState = GameState.gamePaused;
        }
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;
        
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        yield return StartCoroutine(DisplayTextMessage("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! \n\n" +
            "YOU'VE SURVIED THIS DUNGEON LEVEL", Color.white, 5f));
        yield return StartCoroutine(DisplayTextMessage("COLLECT ANY LOOT...THEN PRESS RETURN TO\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 0f));
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        currentDungeonLevelListIdx++;
        PlayDungeonLevel(currentDungeonLevelListIdx);
    }

    private void SavePlayerRankingScore(out string rankText)
    {
        int rank = HighScoreManager.Instance.GetRank(gameScore);
        if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
        {
            rankText = "YOUR SCORE IS RANKED " + rank.ToString("#0") + " IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
            string playerName = GameResources.Instance.currentPlayer.playerName;

            if (playerName == "")
            {
                playerName = playerDetails.playerCharacterName.ToUpper();
            }

            string description = "LEVEL " + (currentDungeonLevelListIdx + 1).ToString() + " - " + GetCurrentDungeonLevel().levelName.ToUpper();
            HighScoreManager.Instance.AddScore(new Score()
            {
                playerName = playerName,
                levelDescription = description,
                playerScore = gameScore
            }, rank);
        }
        else
        {
            rankText = "YOUR SCORE ISN'T RANKED IN THE TOP " + Settings.numberOfHighScoresToSave.ToString("#0");
        }
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        GetPlayer().playerControl.DisablePlayer();

        SavePlayerRankingScore(out string rankText);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
        yield return StartCoroutine(DisplayTextMessage("WELL DONE " + GameResources.Instance.currentPlayer.playerName + "! " +
            "YOU'VE DEFEATED THE DUNGEON", Color.white, 3f));
        yield return StartCoroutine(DisplayTextMessage("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 4f));
        yield return StartCoroutine(DisplayTextMessage("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        yield return new WaitForSeconds(10f);
        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        SavePlayerRankingScore(out string rankText);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayTextMessage("BAD LUCK " + GameResources.Instance.currentPlayer.playerName + "! " +
            "YOU'VE SUCCURED TO THE DUNGEON", Color.white, 2f));
        yield return StartCoroutine(DisplayTextMessage("YOU SCORED " + gameScore.ToString("###,###0") + "\n\n" + rankText, Color.white, 2f));
        yield return StartCoroutine(DisplayTextMessage("PRESS RETURN TO RESTART THE GAME", Color.white, 0f));

        yield return new WaitForSeconds(10f);
        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        currentDungeonLevelListIdx = 0;
        SceneManager.LoadScene("MainMenuScene");
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
        StartCoroutine(DisplayDungeonLevelText());
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        yield return StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIdx + 1).ToString() + "\n\n" +
            dungeonLevelList[currentDungeonLevelListIdx].levelName.ToUpper();
        yield return StartCoroutine(DisplayTextMessage(messageText, Color.white, 2f));
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
        GetPlayer().playerControl.EnablePlayer();
    }

    private IEnumerator DisplayTextMessage(string messageText, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(messageText);
        messageTextTMP.color = textColor;

        float timer = displaySeconds;
        if (timer > 0)
        {
            while (timer > 0 && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else 
        {
            while (!Input.GetKeyDown(KeyCode.Return))
                yield return null;
        }

        yield return null;
        messageTextTMP.SetText("");
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
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
    }
#endif
    #endregion Validation
}
