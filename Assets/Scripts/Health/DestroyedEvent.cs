using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEvetArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied, int points)
    {
        DestroyedEvetArgs destroyedEvetArgs = new DestroyedEvetArgs()
        {
            playerDied = playerDied,
            points = points,
        };
        OnDestroyed?.Invoke(this, destroyedEvetArgs);
    }
}

public class DestroyedEvetArgs : EventArgs
{
    public bool playerDied;
    public int points;
}