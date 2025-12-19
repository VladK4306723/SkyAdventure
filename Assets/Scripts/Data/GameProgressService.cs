using Zenject;

public interface IGameProgressService
{
    SessionData CurrentSession { get; }

    void StartSession();
    void EndSession(GameFinishReason reason);
}

public sealed class GameProgressService : IGameProgressService
{
    [Inject] private IDataManager _dataManager;

    public SessionData CurrentSession { get; private set; }

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
        _dataManager.ApplySession(CurrentSession, reason);
    }
}
