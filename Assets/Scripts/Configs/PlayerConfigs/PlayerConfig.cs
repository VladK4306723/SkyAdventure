using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/PlayerConfig")]
public sealed class PlayerConfig : ScriptableObject, IConfigWithKey<PlayerType>
{
    [SerializeField] private PlayerType _type;
    public PlayerType Key => _type;

    public float TurnSpeed;
    public float SpeedMultiplier;
    public Sprite Sprite;
    
}
