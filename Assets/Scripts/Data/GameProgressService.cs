using System;
using UnityEngine;
using Zenject;

public interface IGameProgressService
{
    SessionData CurrentSession { get; }

    event Action<SessionData, GameFinishReason> SessionFinished;

    void StartSession();
    void EndSession(GameFinishReason reason);
}

public sealed class GameProgressService : IGameProgressService
{
    [Inject] private IDataManager _dataManager;

    public SessionData CurrentSession { get; private set; }

    public event Action<SessionData, GameFinishReason> SessionFinished;
    private readonly string _instanceTag = $"GPS#{Guid.NewGuid().ToString("N")[..6]}";


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
        Debug.Log($"[GPS][END] {_instanceTag} reason={reason} sessionStars={CurrentSession.StarsCollected} time={CurrentSession.FlightTime}");

        CurrentSession.Finish(reason);

        Debug.Log($"[GPS][EVENT] {_instanceTag} invoke SessionFinished");
        SessionFinished?.Invoke(CurrentSession, reason);

        Debug.Log($"[GPS][APPLY] {_instanceTag} apply to IDataManager");
        _dataManager.ApplySession(CurrentSession, reason);
    }

}

