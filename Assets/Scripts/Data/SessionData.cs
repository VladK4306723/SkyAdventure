public class SessionData
{
    public int FlightCost { get; private set; }
    public int StarsCollected { get; private set; }
    public float FlightTime { get; private set; }
    public float MaxMultiplier { get; private set; }
    public float Multiplier { get; private set; }
    public float Danger { get; private set; }
    public bool IsFinished { get; private set; }
    public GameFinishReason FinishReason { get; private set; }

    public void Reset()
    {
        StarsCollected = 0;
        FlightTime = 0f;
        MaxMultiplier = 1f;
        Multiplier = 1f;
        Danger = 0f;
        IsFinished = false;
    }

    public void SetFlightCost(int cost)
    {
        FlightCost = cost;
    }

    public void AddStars(int amount)
    {
        StarsCollected += amount;
    }

    public void UpdateTime(float dt)
    {
        FlightTime += dt;
    }

    public void UpdateDanger(float danger)
    {
            Danger = danger;
    }

    public void UpdateMultiplier(float multiplier)
    {
        Multiplier = multiplier;

        if (multiplier > MaxMultiplier)
            MaxMultiplier = multiplier;
    }

    public void Finish(GameFinishReason reason)
    {
        IsFinished = true;
        FinishReason = reason;
    }
}
