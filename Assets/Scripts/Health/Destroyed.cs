using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    private DestroyedEvent destroyedEvent;

    private void Awake()
    {
        destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable()
    {
        destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEvetArgs destroyedEvetArgs)
    {
        // Deactivate player
        if (destroyedEvetArgs.playerDied)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // Destroy enemies or env objects that are attached to this component
            Destroy(gameObject);
        }
    }
}
