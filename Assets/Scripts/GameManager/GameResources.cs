using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;
    
    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER SELECTION
    [Space(10), Header("PLAYER SELECTION")]
    #endregion
    #region Tooltip
    [Tooltip("The PlayerSelection prefab")]
    #endregion
    public GameObject playerSelectionPrefab;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    #region Tooltip
    [Tooltip("Player details lists - populate with the list with the player details scriptable objects")]
    #endregion
    public List<PlayerDetailsSO> playerDetailsList;
    #region Tooltip
    [Tooltip("The current player scriptable object - this is used to reference to the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayer;

    #region Header MUSIC
    [Space(10), Header("MUSIC")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the music master mixer group")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    #region Tooltip
    [Tooltip("Main menu music scriptable object")]
    #endregion
    public MusicTrackSO mainMenuMusicSO;
    #region Tooltip
    [Tooltip("Music on full snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnFullSnapshot;
    #region Tooltip
    [Tooltip("Music on low snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnLowSnapshot;
    #region Tooltip
    [Tooltip("Music on off snapshot")]
    #endregion
    public AudioMixerSnapshot musicOnOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the sounds master mixer group")]
    #endregion
    public AudioMixerGroup soundMasterMixerGroup;
    #region Tooltip
    [Tooltip("Door open close sound effect")]
    #endregion
    public SoundEffectSO doorOpenCloseSoundEffect;
    #region Tooltip
    [Tooltip("Populate with the table flip sound effect")]
    #endregion
    public SoundEffectSO tableFlipSoundEffectSO;
    #region Tooltip
    [Tooltip("Populate with the chest open sound effect scriptable object")]
    #endregion
    public SoundEffectSO chestOpenSO;
    #region Tooltip
    [Tooltip("Populate with the heart pickup sound effect scriptable object")]
    #endregion
    public SoundEffectSO healthPickupSO;
    #region Tooltip
    [Tooltip("Populate with the weapon pickup sound effect scriptable object")]
    #endregion
    public SoundEffectSO weaponPickupSO;
    #region Tooltip
    [Tooltip("Populate with the ammo pickup sound effect scriptable object")]
    #endregion
    public SoundEffectSO ammoPickupSO;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with the variable Lit Shader")]
    #endregion
    public Shader variableLitShader;
    #region Tooltip
    [Tooltip("Populate with the Materialize Shader")]
    #endregion
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    #region Tooltip
    [Tooltip("Collisions tiles that the enemies can navigate to")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    #region Tooltip
    [Tooltip("Preferred path tile for enemy navigation")]
    #endregion
    public TileBase preferredEnemyPathTile;

    #region UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the heart prefab")]
    #endregion
    public GameObject heartPrefab;
    #region Tooltip
    [Tooltip("Populate with the ammo icon prefab")]
    #endregion
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("The score prefab")]
    #endregion
    public GameObject scorePrefab;

    #region Header CHESTS
    [Space(10)]
    [Header("CHESTS")]
    #endregion
    #region Tooltip
    [Tooltip("Chest item prefab")]
    #endregion
    public GameObject chestItemPrefab;
    #region Tooltip
    [Tooltip("Populate with the heart icon prefab")]
    #endregion
    public Sprite heartIcon;
    #region Tooltip
    [Tooltip("Populate with the bullet icon prefab")]
    #endregion
    public Sprite bulletIcon;

    #region Header MINIMAP
    [Space(10), Header("MINIMAP")]
    #endregion
    #region Tooltip
    [Tooltip("Minimap skull icon prefab")]
    #endregion
    public GameObject minimapSkullPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundMasterMixerGroup), soundMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlipSoundEffectSO), tableFlipSoundEffectSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpenSO), chestOpenSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickupSO), healthPickupSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickupSO), ammoPickupSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickupSO), weaponPickupSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusicSO), mainMenuMusicSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnLowSnapshot), musicOnLowSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnOffSnapshot), musicOnOffSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
    }
#endif
    #endregion
}
