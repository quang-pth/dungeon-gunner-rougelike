using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEvetArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied)
    {
        DestroyedEvetArgs destroyedEvetArgs = new DestroyedEvetArgs()
        {
            playerDied = playerDied
        };
        OnDestroyed?.Invoke(this, destroyedEvetArgs);
    }
}

public class DestroyedEvetArgs : EventArgs
{
    public bool playerDied;
}