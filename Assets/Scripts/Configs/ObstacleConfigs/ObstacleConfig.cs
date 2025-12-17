using UnityEngine;

[CreateAssetMenu(menuName = "Game/Obstacle/ObstacleConfig")]
public sealed class ObstacleConfig : ScriptableObject, IConfigWithKey<ObstacleType>
{
    [SerializeField] private ObstacleType _type;
    public ObstacleType Key => _type;

    public float ActionValue;
    public Sprite Sprite;

}
