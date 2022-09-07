using UnityEngine;
using System;

public static class StaticEventHandler
{
    public static event Action<RoomChangedEventArgs> OnRoomChanged;
    public static void CallRoomChangedEvent(Room room) {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }

    public static event Action<RoomEnemiesDefeatedEventArgs> OnRoomEnemiesDefeated;
    public static void CallOnRoomEnemiesDefeated(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedEventArgs() { room = room });
    }

    public static event Action<PointsScoredArgs> OnPointsScored;
    public static void CallOnPointsScoredEvent(int points)
    {
        OnPointsScored?.Invoke(new PointsScoredArgs() { points = points });
    }

    public static event Action<ScoreChangedArgs> OnScoreChanged;
    public static void CallOnScoreChangedEvent(long score, long multiplier)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score = score, multiplier = multiplier });
    }
    
    public static event Action<MultiplierArgs> OnMultiplier;
    public static void CallOnMultiplierEvent(bool multiplier)
    {
        OnMultiplier?.Invoke(new MultiplierArgs() { multiplier = multiplier });
    }
}

public class RoomChangedEventArgs : EventArgs {
    public Room room;
}

public class RoomEnemiesDefeatedEventArgs : EventArgs {
    public Room room;
}

public class PointsScoredArgs : EventArgs
{
    public int points;
}

public class ScoreChangedArgs : EventArgs
{
    public long score;
    public long multiplier;
}

public class MultiplierArgs : EventArgs
{
    public bool multiplier;
}