public sealed class PlayerModel
{
    public float TurnSpeed { get; }
    public float SpeedMultiplier { get; }

    public PlayerModel(PlayerConfig config)
    {
        TurnSpeed = config.TurnSpeed;
        SpeedMultiplier = config.SpeedMultiplier;
    }
}
