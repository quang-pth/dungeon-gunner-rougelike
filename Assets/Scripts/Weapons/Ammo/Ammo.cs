using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate with the child TrailRenderer component")]
    #endregion
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer = 1f;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        // Charging the ammo
        if (ammoChargeTimer > 0f) {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        // Change the ammo material from charging material to shoot material
        else if (!isAmmoMaterialSet) {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // Move the ammo each frame towards the fire direction vector
        Vector3 distanceVector = (fireDirectionVector.normalized * ammoSpeed) * Time.deltaTime;
        transform.position += distanceVector;

        // Disable the ammo if it reached the ammo range
        ammoRange -= distanceVector.magnitude;
        if (ammoRange <= 0f) {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Display ammo hit effect before destroy it
        PlayAmmoHitEffect();
        // Disable the ammo if it coolides with other object
        DisableAmmo();
    }

    private void PlayAmmoHitEffect() {
        if (ammoDetails.ammoHitEffectSO != null && ammoDetails.ammoHitEffectSO.ammoHitEffectPrefab != null) {
            AmmoHitEffect ammoHitEffect = PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffectSO.ammoHitEffectPrefab,
                            transform.position, Quaternion.identity) as AmmoHitEffect;
            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffectSO);
            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, 
            Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false) 
    {
        #region Ammo
        this.ammoDetails = ammoDetails;

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // Set init ammo material depending on if there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f) {
            this.ammoChargeTimer = ammoDetails.ammoChargeTime;

            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);

            isAmmoMaterialSet = false;
        } 
        else {
            this.ammoChargeTimer = 0f;

            SetAmmoMaterial(ammoDetails.ammoMaterial);

            isAmmoMaterialSet = true;
        }

        this.ammoRange = ammoDetails.ammoRange;

        this.ammoSpeed = ammoSpeed;

        this.overrideAmmoMovement = overrideAmmoMovement;

        // Active the ammo gameobject
        gameObject.SetActive(true);

        #endregion Ammo

        #region Ammo Trail

        if (ammoDetails.isAmmoTrail) {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion Ammo Trail
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetailsSO, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector) {
        float randomSpread = Random.Range(ammoDetailsSO.ammoSpreadMin, ammoDetailsSO.ammoSpreadMax);
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        // Change fire aim direction angle if enemies are too closed to the player
        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance) {
            fireDirectionAngle = aimAngle;
        }
        else {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += spreadToggle * randomSpread;

        // Set ammo ratation
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        fireDirectionVector = HelperUtilities.GetDirectionFromAngle(fireDirectionAngle);
    }

    private void DisableAmmo() {
        gameObject.SetActive(false);
    }
    
    private void SetAmmoMaterial(Material material) {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject() {
        return gameObject;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }
#endif
    #endregion
}
