using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the image component on the child WeaponImage component")]
    #endregion
    [SerializeField] private Image weaponImage;

    #region Tooltip
    [Tooltip("Populate with the Transform from the child AmmoHolder gameobject")]
    #endregion
    [SerializeField] private Transform ammoHolderTransform;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI reloadText;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI ammoRemainingText;

    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameobject")]
    #endregion
    [SerializeField] private TextMeshProUGUI weaponNameText;

    #region Tooltip
    [Tooltip("Populate with the RectTransform of the child gameobject ReloadBar")]
    #endregion
    [SerializeField] private Transform reloadBar;

    #region Tooltip
    [Tooltip("Populate with the Image component of the child gameobject BarImage")]
    #endregion
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake() {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable() {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable() {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start() {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs) {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs) {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void WeaponFired(Weapon weapon) {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs) {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs) {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon) {
        if (player.activeWeapon.GetCurrentWeapon() == weapon) {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void SetActiveWeapon(Weapon weapon) {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isWeaponReloading) {
            UpdateWeaponReloadBar(weapon);
        }
        else {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    private void UpdateAmmoText(Weapon weapon) {
        string textToDisplay = "";
        
        if (weapon.weaponDetails.hasInfiniteAmmo) {
            textToDisplay = "INFINITE AMMO";
        }
        else {
            textToDisplay = weapon.weaponRemainingAmmo.ToString() + "/" + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }

        ammoRemainingText.text = textToDisplay;
    }

    private void UpdateReloadText(Weapon weapon) {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading)) {
            reloadText.color = Color.red;
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();
            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextCoroutine(weapon));
        }
        else {
            StopBlinkingReloadText();
        }
    }

    private IEnumerator StartBlinkingReloadTextCoroutine(Weapon weapon) {
        while (true) {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText() {
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine() {
        if (blinkingReloadTextCoroutine != null) {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon) {
        ClearAmmoLoadedIcons();

        for (int idx = 0; idx < weapon.weaponClipRemainingAmmo; idx++) {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Settings.uiAmmoIconSpacing * idx);
            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons() {
        foreach (GameObject ammoIcon in ammoIconList) {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void ResetWeaponReloadBar() {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;

        barImage.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void UpdateWeaponReloadBar(Weapon weapon) {
        if (weapon.weaponDetails.hasInfiniteClipCapacity) return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private void StopReloadWeaponCoroutine() {
        if (reloadWeaponCoroutine != null) {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon) {
        barImage.color = Color.red;

        while (weapon.isWeaponReloading) {
            // Get the bar ratio
            float barFill = weapon.weaponReloadTimer / weapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetailsSO) {
        weaponImage.sprite = weaponDetailsSO.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon) {
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetails.weaponName.ToUpper();
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion
}