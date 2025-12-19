using System;
using UnityEngine;
using Zenject;

public interface IGameProgressService
{
    SessionData CurrentSession { get; }

    event Action<SessionData, GameFinishReason, SessionResult> SessionFinished;

    void StartSession();
    void EndSession(GameFinishReason reason);
}

public sealed class GameProgressService : IGameProgressService
{
    [Inject] private IDataManager _dataManager;

    public SessionData CurrentSession { get; private set; }

    public event Action<SessionData, GameFinishReason, SessionResult> SessionFinished;


    public GameProgressService()
    {
        CurrentSession = new SessionData();
    }

    public void StartSession()
    {
        CurrentSession.Reset();
    }

    public void EndSession(GameFinishReason reason)
    {
        CurrentSession.Finish(reason);

        var result = _dataManager.ApplySession(CurrentSession, reason);

        SessionFinished?.Invoke(CurrentSession, reason, result);
    }

}


