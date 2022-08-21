using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("Set this to the color to be used for the materializtion effect")]
    #endregion
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;
    #region Tooltip
    [Tooltip("Set this to the time is will take to materialize the chest")]
    #endregion
    [SerializeField] private float materializeTime = 3f;
    #region Tooltip
    [Tooltip("Populate withItemSpawnPoint transform")]
    #endregion
    [SerializeField] private Transform itemSpawnPoint;
    private int healthPercent;
    public WeaponDetailsSO weaponDetailsSO;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextMeshPro;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextMeshPro = GetComponentInChildren<TextMeshPro>();
    }

    private void Start()
    {
        Intialize(true, 50, weaponDetailsSO, 60);
    }

    public void Intialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetailsSO,
        int ammoPercent)
    {
        this.healthPercent = healthPercent;
        this.weaponDetailsSO = weaponDetailsSO;
        this.ammoPercent = ammoPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    private void EnableChest()
    {
        isEnabled = true;
    }

    public void UseItem()
    {
        if (!isEnabled) return;

        switch(chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            case ChestState.empty:
                return;
            default:
                return;
        }
    }

    private void OpenChest()
    {
        animator.SetBool(Settings.use, true);
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpenSO);

        if (weaponDetailsSO != null)
        {
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetailsSO))
            {
                weaponDetailsSO = null;
            }
        }

        UpdateChestState();
    }

    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0)
        {
            chestState = ChestState.ammoItem;
            InstantiateAmmoItem();
        }
        else if (weaponDetailsSO)
        {
            chestState = ChestState.weaponItem;
            InstantiateWeaponItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);
        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem();
        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%",
            itemSpawnPoint.position, materializeColor);
    }

    private void InstantiateAmmoItem()
    {
        InstantiateItem();
        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%",
            itemSpawnPoint.position, materializeColor);
    }

    private void InstantiateWeaponItem()
    {
        InstantiateItem();
        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetailsSO.weaponSprite,
            weaponDetailsSO.weaponName, itemSpawnPoint.position, materializeColor);
    }

    private void CollectHealthItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickupSO);
        healthPercent = 0;
        Destroy(chestItemGameObject);
        UpdateChestState();
    }

    private void CollectAmmoItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();
        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickupSO);
        ammoPercent = 0;
        Destroy(chestItemGameObject);
        UpdateChestState();
    }

    private void CollectWeaponItem()
    {
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetailsSO))
        {
            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetailsSO);
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickupSO);
        }
        else
        {
            StartCoroutine(DisplayMessage("Weapon\nalready\nequipped", 5f));
        }

        weaponDetailsSO = null;
        Destroy(chestItemGameObject);
        UpdateChestState();
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
    {
        messageTextMeshPro.text = messageText;
        yield return new WaitForSeconds(messageDisplayTime);
        messageTextMeshPro.text = "";
     }
}

