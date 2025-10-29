using System;

public static class GameEvents
{
    public static Action OnGameStarted;
    public static Action OnGameRestarted;
    public static Action OnGameOver;
    public static Action OnGamePaused;
    public static Action OnGameResumed;
}