using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHeartList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBars(healthEventArgs.healthPercent);
    }

    private void ClearHealthBars()
    {
        foreach(GameObject heartIcon in healthHeartList)
        {
            Destroy(heartIcon);
        }

        healthHeartList.Clear();
    }
    private void SetHealthBars(float healthPercent)
    {
        ClearHealthBars();

        // Get the number of heart icons to display
        int healthHearts = Mathf.CeilToInt(healthPercent * 100f / 20f);

        for (int idx = 0; idx < healthHearts; idx++)
        {
            GameObject heartIcon = Instantiate(GameResources.Instance.heartPrefab, transform);
            // Offset the icon along the x-axis, leave the y-axis intact
            heartIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * idx, 0f);

            healthHeartList.Add(heartIcon);
        }
    }
}
