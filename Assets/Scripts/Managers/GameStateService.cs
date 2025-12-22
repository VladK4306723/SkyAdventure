using System;

public interface IGameStateService
{
    GameState State { get; }
    event Action<GameState> StateChanged;

    bool IsPlaying { get; }

    void SetState(GameState state);
}

public sealed class GameStateService : IGameStateService
{
    public GameState State { get; private set; } = GameState.Paused;

    public bool IsPlaying => State == GameState.Playing;

    public event Action<GameState> StateChanged;

    public void SetState(GameState state)
    {
        if (State == state)
            return;

        State = state;
        StateChanged?.Invoke(State);
    }
}