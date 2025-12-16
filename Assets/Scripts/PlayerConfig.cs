using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/PlayerConfig")]
public sealed class PlayerConfig : ScriptableObject
{
    public PlayerType Type;
    public float TurnSpeed;
    public float SpeedMultiplier;
    public Sprite Sprite;
}
