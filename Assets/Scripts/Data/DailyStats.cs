[System.Serializable]
public sealed class DailyStats
{
    public string Date;

    public int TotalStars;     
    public int StarsCollected;   
    public int CoinsSpent;

    public int Sessions;
    public int SuccessfulSessions;

    public float SuccessRate =>
        Sessions == 0 ? 0f : (float)SuccessfulSessions / Sessions;
}
