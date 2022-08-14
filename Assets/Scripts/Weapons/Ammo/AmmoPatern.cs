using UnityEngine;

public class AmmoPatern : MonoBehaviour, IFireable
{
    #region Tooltip
    [Tooltip("Populate the array with the child ammo gameobjects")]
    #endregion
    [SerializeField] private Ammo[] ammoArray;

    private float ammoRange;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetailsSO;
    private float ammoChargeTimer;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, 
        float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        this.ammoDetailsSO = ammoDetails;
        this.ammoSpeed = ammoSpeed;

        SetFireDirection(ammoDetailsSO, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        ammoRange = ammoDetailsSO.ammoRange;
        gameObject.SetActive(true);
        foreach (Ammo ammo in ammoArray)
        {
            ammo.InitialiseAmmo(ammoDetailsSO, aimAngle, weaponAimAngle, ammoSpeed, 
                weaponAimDirectionVector, true);
        }

        if (ammoDetailsSO.ammoChargeTime > 0f)
        {
            ammoChargeTimer = ammoDetailsSO.ammoChargeTime;
        }
        else
        {
            ammoChargeTimer = 0f;
        }
    }

    private void Update()
    {
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        
        Vector3 distanceVector = ammoSpeed * Time.deltaTime * fireDirectionVector;
        transform.position += distanceVector;
        transform.Rotate(new Vector3(0f, 0f, ammoDetailsSO.ammoRotationSpeed * Time.deltaTime));
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetFireDirection(AmmoDetailsSO ammoDetailsSO, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        float randomSpread = Random.Range(ammoDetailsSO.ammoSpreadMin, ammoDetailsSO.ammoSpreadMax);
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += spreadToggle * randomSpread;

        fireDirectionVector = HelperUtilities.GetDirectionFromAngle(fireDirectionAngle);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
    }
#endif
    #endregion
}